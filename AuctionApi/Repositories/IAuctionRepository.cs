using AuctionApi.Models;

namespace AuctionApi.Repositories;

// Repository-kontrakt: endast databasoperationer (EF Core)
public interface IAuctionRepository
{
    // Create/Update
    Task AddAuctionAsync(Auction auction);
    Task<Auction?> GetAuctionByIdForUpdateAsync(int id);
    Task<bool> AuctionHasBidsAsync(int auctionId);
    Task<decimal?> GetHighestBidAsync(int auctionId);
    Task SaveChangesAsync();

    // Read/Search
    Task<List<Auction>> SearchAuctionsAsync(string? title, bool includeClosed, DateTime now);
    Task<Auction?> GetAuctionByIdForDetailsAsync(int id);
    Task<Auction?> GetAuctionByIdAsync(int id);

    // Bids list
    Task<List<object>> GetBidsForAuctionAsync(int auctionId);
}