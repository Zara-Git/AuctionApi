using AuctionApi.Models;
using AuctionApi.Repositories;
using AuctionApi.Services.Interfaces;

namespace AuctionApi.Services;

public class BidService : IBidService
{
    private readonly IBidRepository _repo;

    public BidService(IBidRepository repo)
    {
        _repo = repo;
    }

    public async Task<int> CreateBidAsync(int userId, int auctionId, decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than 0.");

        var auction = await _repo.GetAuctionByIdAsync(auctionId);
        if (auction == null)
            throw new Exception("Auction not found.");

        if (auction.IsDisabled)
            throw new Exception("Auction is disabled.");

        var now = DateTime.UtcNow;

        if (auction.StartDate > now)
            throw new Exception("Auction has not started yet.");

        if (auction.EndDate <= now)
            throw new Exception("Auction is closed.");

        var user = await _repo.GetUserByIdAsync(userId);
        if (user == null)
            throw new Exception("User not found.");

        if (!user.IsActive)
            throw new UnauthorizedAccessException("Account is disabled.");

        if (auction.CreatedByUserId == userId)
            throw new Exception("You cannot bid on your own auction.");

        var highest = await _repo.GetHighestBidAmountAsync(auctionId);
        var min = highest ?? auction.StartingPrice;

        if (amount <= min)
            throw new Exception($"Bid is too low. Must be higher than {min}.");

        var bid = new Bid
        {
            AuctionId = auctionId,
            UserId = userId,
            Amount = amount,
            CreatedAt = now
        };

        await _repo.AddBidAsync(bid);
        await _repo.SaveChangesAsync();

        return bid.Id;
    }

    public async Task UndoBidAsync(int userId, int bidId)
    {
        var bid = await _repo.GetBidByIdAsync(bidId);
        if (bid == null)
            throw new Exception("Bid not found.");

        if (bid.UserId != userId)
            throw new UnauthorizedAccessException("You can only undo your own bid.");

        var auction = await _repo.GetAuctionByIdAsync(bid.AuctionId);
        if (auction == null)
            throw new Exception("Auction not found.");

        var now = DateTime.UtcNow;

        if (auction.EndDate <= now)
            throw new Exception("Auction is closed.");

        if (auction.IsDisabled)
            throw new Exception("Auction is disabled.");

        var latestBidId = await _repo.GetLatestBidIdAsync(bid.AuctionId);

        if (latestBidId != bid.Id)
            throw new Exception("Only the latest bid can be undone.");

        _repo.RemoveBid(bid);
        await _repo.SaveChangesAsync();
    }
}