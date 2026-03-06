using AuctionApi.DTOs.Auctions;
using AuctionApi.Models;
using AuctionApi.Repositories;
using AuctionApi.Services.Interfaces;

namespace AuctionApi.Services;

// Service: validering + regler (ägare, bud-regler, open/closed)
public class AuctionService : IAuctionService
{
    private readonly IAuctionRepository _repo;
    public AuctionService(IAuctionRepository repo) => _repo = repo;

    public async Task<(bool ok, int status, string? error, object? result)> CreateAsync(int userId, CreateAuctionRequestDto dto)
    {
        if (dto is null) return (false, 400, "Request body is required.", null);
        if (string.IsNullOrWhiteSpace(dto.Title)) return (false, 400, "Title is required.", null);
        if (dto.StartingPrice < 0) return (false, 400, "StartingPrice cannot be negative.", null);
        if (dto.EndDate <= dto.StartDate) return (false, 400, "EndDate must be after StartDate.", null);

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

        await _repo.AddAuctionAsync(auction);
        return (true, 200, null, new { auction.Id });
    }

    public async Task<(bool ok, int status, string? error, object? result)> SearchAsync(string? title, bool includeClosed)
    {
        var now = DateTime.UtcNow;
        var auctions = await _repo.SearchAuctionsAsync(title, includeClosed, now);

        var result = auctions.Select(a => new AuctionListItemDto
        {
            Id = a.Id,
            Title = a.Title,
            StartingPrice = a.StartingPrice,
            EndDate = a.EndDate,
            SellerName = a.CreatedByUser!.Name
        }).ToList();

        return (true, 200, null, result);
    }

    public async Task<(bool ok, int status, string? error, object? result)> GetByIdAsync(int id)
    {
        var a = await _repo.GetAuctionByIdForDetailsAsync(id);
        if (a is null) return (false, 404, "Auction not found.", null);

        var highestBid = await _repo.GetHighestBidAsync(id);
        var now = DateTime.UtcNow;

        var dto = new AuctionDetailsDto
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
        };

        return (true, 200, null, dto);
    }

    public async Task<(bool ok, int status, string? error, object? result)> GetBidsAsync(int id)
    {
        var now = DateTime.UtcNow;
        var auction = await _repo.GetAuctionByIdAsync(id);
        if (auction is null) return (false, 404, "Auction not found.", null);

        var isOpen = !auction.IsDisabled && auction.StartDate <= now && auction.EndDate > now;
        if (!isOpen) return (false, 400, "Bids are not available for closed auctions.", null);

        var bids = await _repo.GetBidsForAuctionAsync(id);
        return (true, 200, null, bids);
    }

    public async Task<(bool ok, int status, string? error, object? result)> UpdateAsync(int userId, int id, UpdateAuctionRequestDto dto)
    {
        if (dto is null) return (false, 400, "Request body is required.", null);

        var auction = await _repo.GetAuctionByIdForUpdateAsync(id);
        if (auction is null) return (false, 404, "Auction not found.", null);

        // Bara skaparen får uppdatera
        if (auction.CreatedByUserId != userId)
            return (false, 403, "Only the creator can update this auction.", null);

        // Pris får inte ändras om bud finns
        var hasBids = await _repo.AuctionHasBidsAsync(id);
        if (hasBids && dto.StartingPrice != auction.StartingPrice)
            return (false, 400, "Cannot change starting price after bids have been placed.", null);

        auction.Title = dto.Title;
        auction.Description = dto.Description;
        auction.StartDate = dto.StartDate;
        auction.EndDate = dto.EndDate;

        if (!hasBids)
            auction.StartingPrice = dto.StartingPrice;

        await _repo.SaveChangesAsync();
        return (true, 200, null, auction);
    }
}