namespace AuctionApi.Services.Interfaces;

// Service-kontrakt: business logic (regler) för admin-funktioner
public interface IAdminService
{
    Task<(bool ok, int status, string? error)> DisableAuctionAsync(int id);
    Task<(bool ok, int status, string? error)> EnableAuctionAsync(int id);

    Task<(bool ok, int status, string? error)> DisableUserAsync(int id);
    Task<(bool ok, int status, string? error)> EnableUserAsync(int id);
}