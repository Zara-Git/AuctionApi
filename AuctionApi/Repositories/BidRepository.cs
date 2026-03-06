using AuctionApi.Data;
using AuctionApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AuctionApi.Repositories;

public class BidRepository : IBidRepository
{
    private readonly AuctionDbContext _db;

    public BidRepository(AuctionDbContext db)
    {
        _db = db;
    }

    public async Task<Auction?> GetAuctionByIdAsync(int auctionId)
    {
        return await _db.Auctions.FirstOrDefaultAsync(a => a.Id == auctionId);
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _db.Users.FindAsync(userId);
    }

    public async Task<Bid?> GetBidByIdAsync(int bidId)
    {
        return await _db.Bids.FirstOrDefaultAsync(b => b.Id == bidId);
    }

    public async Task<decimal?> GetHighestBidAmountAsync(int auctionId)
    {
        return await _db.Bids
            .Where(b => b.AuctionId == auctionId)
            .MaxAsync(b => (decimal?)b.Amount);
    }

    public async Task<int> GetLatestBidIdAsync(int auctionId)
    {
        return await _db.Bids
            .Where(b => b.AuctionId == auctionId)
            .OrderByDescending(b => b.CreatedAt)
            .ThenByDescending(b => b.Id)
            .Select(b => b.Id)
            .FirstOrDefaultAsync();
    }

    public async Task AddBidAsync(Bid bid)
    {
        await _db.Bids.AddAsync(bid);
    }

    public void RemoveBid(Bid bid)
    {
        _db.Bids.Remove(bid);
    }

    public async Task SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
    }
}