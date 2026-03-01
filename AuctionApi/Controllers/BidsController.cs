using AuctionApi.Data;
using AuctionApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AuctionApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BidsController : ControllerBase
{
    private readonly AuctionDbContext _db;
    public BidsController(AuctionDbContext db) => _db = db;

    public record CreateBidDto(int AuctionId, decimal Amount);
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(CreateBidDto dto)
    {
        if (dto.Amount <= 0)
            return BadRequest("Amount must be greater than 0.");

        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userId = int.Parse(userIdStr!);

        var auction = await _db.Auctions.FirstOrDefaultAsync(a => a.Id == dto.AuctionId);
        if (auction == null) return NotFound("Auction not found.");

        if (auction.IsDisabled) return BadRequest("Auction is disabled.");

        var now = DateTime.UtcNow;

        if (auction.StartDate > now)
            return BadRequest("Auction has not started yet.");

        if (auction.EndDate <= now)
            return BadRequest("Auction is closed.");

        // VG: användaren måste finnas + vara aktiv
        var user = await _db.Users.FindAsync(userId);
        if (user == null) return BadRequest("User not found.");
        if (!user.IsActive) return Unauthorized("Account is disabled.");

        if (auction.CreatedByUserId == userId)
            return BadRequest("You cannot bid on your own auction.");

        var highest = await _db.Bids
            .Where(b => b.AuctionId == dto.AuctionId)
            .MaxAsync(b => (decimal?)b.Amount);

        var min = highest ?? auction.StartingPrice;

        if (dto.Amount <= min)
            return BadRequest($"Bid is too low. Must be higher than {min}.");

        var bid = new Bid
        {
            AuctionId = dto.AuctionId,
            UserId = userId,
            Amount = dto.Amount,
            CreatedAt = now
        };

        _db.Bids.Add(bid);
        await _db.SaveChangesAsync();

        return Ok(new { bid.Id });
    }
    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Undo(int id)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userId = int.Parse(userIdStr!);

        var bid = await _db.Bids.FirstOrDefaultAsync(b => b.Id == id);
        if (bid == null) return NotFound("Bid not found.");

        // Bara den som lade budet får ångra (rimligt och brukar krävas)
        if (bid.UserId != userId) return Forbid();

        var auction = await _db.Auctions.FirstOrDefaultAsync(a => a.Id == bid.AuctionId);
        if (auction == null) return NotFound("Auction not found.");

        var now = DateTime.UtcNow;
        if (auction.EndDate <= now) return BadRequest("Auction is closed.");
        if (auction.IsDisabled) return BadRequest("Auction is disabled.");

        // Kontroll: måste vara senaste budet på auktionen
        // (vi tar senaste via CreatedAt, och fallback på Id om samma timestamp)
        var latestBidId = await _db.Bids
            .Where(b => b.AuctionId == bid.AuctionId)
            .OrderByDescending(b => b.CreatedAt)
            .ThenByDescending(b => b.Id)
            .Select(b => b.Id)
            .FirstOrDefaultAsync();

        if (latestBidId != bid.Id)
            return BadRequest("Only the latest bid can be undone.");

        _db.Bids.Remove(bid);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}