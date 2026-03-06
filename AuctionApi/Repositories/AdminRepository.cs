using AuctionApi.Data;
using AuctionApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AuctionApi.Repositories;

// Repository: pratar med databasen via AuctionDbContext
public class AdminRepository : IAdminRepository
{
    private readonly AuctionDbContext _db;
    public AdminRepository(AuctionDbContext db) => _db = db;

    public Task<Auction?> GetAuctionByIdAsync(int id)
        => _db.Auctions.FirstOrDefaultAsync(a => a.Id == id);

    public Task<User?> GetUserByIdAsync(int id)
        => _db.Users.FirstOrDefaultAsync(u => u.Id == id);

    public Task SaveChangesAsync()
        => _db.SaveChangesAsync();
}