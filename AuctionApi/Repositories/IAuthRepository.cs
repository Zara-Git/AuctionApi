using AuctionApi.Models;

namespace AuctionApi.Repositories;

public interface IAuthRepository
{
    Task<bool> EmailExistsAsync(string email);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(int id);
    Task AddUserAsync(User user);
    Task SaveChangesAsync();
}