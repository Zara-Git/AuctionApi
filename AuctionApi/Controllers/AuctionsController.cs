using AuctionApi.Data;
using AuctionApi.DTOs.Auctions;
using AuctionApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AuctionApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuctionsController : ControllerBase
{
    private readonly AuctionDbContext _db;

    public AuctionsController(AuctionDbContext db) => _db = db;

    // POST: api/auctions
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAuctionRequestDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title)) return BadRequest("Title is required.");
        if (dto.StartingPrice < 0) return BadRequest("StartingPrice cannot be negative.");
        if (dto.EndDate <= dto.StartDate) return BadRequest("EndDate must be after StartDate.");

        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userId = int.Parse(userIdStr!);

        var auction = new Auction
        {
            Title = dto.Title.Trim(),
            Description = dto.Description?.Trim() ?? "",
            StartingPrice = dto.StartingPrice,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            CreatedByUserId = userId,
            IsDisabled = false
        };

        _db.Auctions.Add(auction);
        await _db.SaveChangesAsync();

        return Ok(new { auction.Id });
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string? title, [FromQuery] bool includeClosed = false)
    {
        var now = DateTime.UtcNow;

        var query = _db.Auctions
            .AsNoTracking()
            .Include(a => a.CreatedByUser)
            .Where(a => !a.IsDisabled && a.StartDate <= now);

        if (!includeClosed)
            query = query.Where(a => a.EndDate > now);

        if (!string.IsNullOrWhiteSpace(title))
        {
            var t = title.Trim();
            query = query.Where(a => EF.Functions.Like(a.Title, $"%{t}%"));
        }

        var auctions = await query
            .OrderBy(a => a.EndDate)
            .Select(a => new AuctionListItemDto
            {
                Id = a.Id,
                Title = a.Title,
                StartingPrice = a.StartingPrice,
                EndDate = a.EndDate,
                SellerName = a.CreatedByUser!.Name
            })
            .ToListAsync();

        return Ok(auctions);
    }
    // GET: api/auctions/{id}  ( HighestBid + IsOpen)
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var a = await _db.Auctions
            .AsNoTracking()
            .Include(x => x.CreatedByUser)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (a == null) return NotFound("Auction not found.");

        var highestBid = await _db.Bids
            .AsNoTracking()
            .Where(b => b.AuctionId == id)
            .MaxAsync(b => (decimal?)b.Amount);

        var now = DateTime.UtcNow;

        return Ok(new AuctionDetailsDto
        {
            Id = a.Id,
            Title = a.Title,
            Description = a.Description,
            StartingPrice = a.StartingPrice,
            StartDate = a.StartDate,
            EndDate = a.EndDate,
            CreatedByUserId = a.CreatedByUserId,
            SellerName = a.CreatedByUser!.Name,

            IsOpen = !a.IsDisabled && a.StartDate <= now && a.EndDate > now,
            HighestBid = highestBid
        });
    }

    // GET: api/auctions/{id}/bids
    [HttpGet("{id:int}/bids")]
    public async Task<IActionResult> GetBids(int id)
    {
        var now = DateTime.UtcNow;

        var auction = await _db.Auctions
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);

        if (auction == null) return NotFound("Auction not found.");

        var isOpen = !auction.IsDisabled && auction.StartDate <= now && auction.EndDate > now;
        if (!isOpen)
            return BadRequest("Bids are not available for closed auctions.");

        var bids = await _db.Bids
            .AsNoTracking()
            .Where(b => b.AuctionId == id)
            .Include(b => b.User)
            .OrderByDescending(b => b.Amount)
            .Select(b => new
            {
                b.Id,
                b.Amount,
                b.CreatedAt,
                b.UserId,
                BidderName = b.User!.Name
            })
            .ToListAsync();

        return Ok(bids);
    }
    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateAuctionRequestDto dto)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userId = int.Parse(userIdStr!);

        var auction = await _db.Auctions.FirstOrDefaultAsync(a => a.Id == id);
        if (auction == null) return NotFound("Auction not found.");

        // Bara skaparen får uppdatera
        if (auction.CreatedByUserId != userId)
            return Forbid();

        //pris får inte ändras om bud finns
        var hasBids = await _db.Bids.AnyAsync(b => b.AuctionId == id);
        if (hasBids && dto.StartingPrice != auction.StartingPrice)
            return BadRequest("Cannot change starting price after bids have been placed.");

        auction.Title = dto.Title;
        auction.Description = dto.Description;
        auction.StartDate = dto.StartDate;
        auction.EndDate = dto.EndDate;

        // bara om inga bud: tillåt ändra starting price
        if (!hasBids)
            auction.StartingPrice = dto.StartingPrice;

        await _db.SaveChangesAsync();
        return Ok(auction);
    }
}