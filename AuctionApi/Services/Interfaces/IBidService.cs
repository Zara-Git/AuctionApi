namespace AuctionApi.Services.Interfaces;

public interface IBidService
{
    Task<int> CreateBidAsync(int userId, int auctionId, decimal amount);
    Task UndoBidAsync(int userId, int bidId);
}