using AuctionApi.Repositories;
using AuctionApi.Services.Interfaces;

namespace AuctionApi.Services;

// Service: innehåller regler + använder repo för DB-access
public class AdminService : IAdminService
{
    private readonly IAdminRepository _repo;
    public AdminService(IAdminRepository repo) => _repo = repo;

    // Inaktiverar auktion (syns ej i sök)
    public async Task<(bool ok, int status, string? error)> DisableAuctionAsync(int id)
    {
        var auction = await _repo.GetAuctionByIdAsync(id);
        if (auction is null) return (false, 404, "Auction not found.");

        auction.IsDisabled = true;
        await _repo.SaveChangesAsync();
        return (true, 204, null);
    }

    // Aktiverar auktion igen
    public async Task<(bool ok, int status, string? error)> EnableAuctionAsync(int id)
    {
        var auction = await _repo.GetAuctionByIdAsync(id);
        if (auction is null) return (false, 404, "Auction not found.");

        auction.IsDisabled = false;
        await _repo.SaveChangesAsync();
        return (true, 204, null);
    }

    // Inaktiverar användare (kan ej logga in/bjuda)
    public async Task<(bool ok, int status, string? error)> DisableUserAsync(int id)
    {
        var user = await _repo.GetUserByIdAsync(id);
        if (user is null) return (false, 404, "User not found.");

        user.IsActive = false;
        await _repo.SaveChangesAsync();
        return (true, 204, null);
    }

    // Aktiverar användare igen
    public async Task<(bool ok, int status, string? error)> EnableUserAsync(int id)
    {
        var user = await _repo.GetUserByIdAsync(id);
        if (user is null) return (false, 404, "User not found.");

        user.IsActive = true;
        await _repo.SaveChangesAsync();
        return (true, 204, null);
    }
}