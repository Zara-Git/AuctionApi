using AuctionApi.Models;

namespace AuctionApi.Repositories;

public interface IBidRepository
{
    Task<Auction?> GetAuctionByIdAsync(int auctionId);
    Task<User?> GetUserByIdAsync(int userId);
    Task<Bid?> GetBidByIdAsync(int bidId);
    Task<decimal?> GetHighestBidAmountAsync(int auctionId);
    Task<int> GetLatestBidIdAsync(int auctionId);
    Task AddBidAsync(Bid bid);
    void RemoveBid(Bid bid);
    Task SaveChangesAsync();
}