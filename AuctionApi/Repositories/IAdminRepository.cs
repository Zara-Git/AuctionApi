using AuctionApi.Models;

namespace AuctionApi.Repositories;

// Repository-kontrakt: endast databasoperationer (EF Core)
public interface IAdminRepository
{
    // Hämtar auktion (för enable/disable)
    Task<Auction?> GetAuctionByIdAsync(int id);

    // Hämtar användare (för enable/disable)
    Task<User?> GetUserByIdAsync(int id);

    // Sparar ändringar i databasen
    Task SaveChangesAsync();
}