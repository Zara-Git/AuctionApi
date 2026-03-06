using AuctionApi.Data;
using AuctionApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AuctionApi.Repositories;

// Repository: pratar med databasen via AuctionDbContext
public class AuctionRepository : IAuctionRepository
{
    private readonly AuctionDbContext _db;
    public AuctionRepository(AuctionDbContext db) => _db = db;

    public async Task AddAuctionAsync(Auction auction)
    {
        _db.Auctions.Add(auction);
        await _db.SaveChangesAsync();
    }

    // Tracking behövs för update
    public Task<Auction?> GetAuctionByIdForUpdateAsync(int id)
        => _db.Auctions.FirstOrDefaultAsync(a => a.Id == id);

    public Task SaveChangesAsync() => _db.SaveChangesAsync();

    public Task<bool> AuctionHasBidsAsync(int auctionId)
        => _db.Bids.AnyAsync(b => b.AuctionId == auctionId);

    public Task<decimal?> GetHighestBidAsync(int auctionId)
        => _db.Bids.AsNoTracking()
            .Where(b => b.AuctionId == auctionId)
            .MaxAsync(b => (decimal?)b.Amount);

    public Task<Auction?> GetAuctionByIdForDetailsAsync(int id)
        => _db.Auctions.AsNoTracking()
            .Include(a => a.CreatedByUser)
            .FirstOrDefaultAsync(a => a.Id == id);

    public Task<Auction?> GetAuctionByIdAsync(int id)
        => _db.Auctions.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);

    public async Task<List<Auction>> SearchAuctionsAsync(string? title, bool includeClosed, DateTime now)
    {
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

        return await query.OrderBy(a => a.EndDate).ToListAsync();
    }

    public async Task<List<object>> GetBidsForAuctionAsync(int auctionId)
    {
        return await _db.Bids
            .AsNoTracking()
            .Where(b => b.AuctionId == auctionId)
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
            .Cast<object>()
            .ToListAsync();
    }
}