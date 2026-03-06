using AuctionApi.DTOs.Auctions;

namespace AuctionApi.Services.Interfaces;

// Service-kontrakt: business logic + regler
public interface IAuctionService
{
    Task<(bool ok, int status, string? error, object? result)> CreateAsync(int userId, CreateAuctionRequestDto dto);
    Task<(bool ok, int status, string? error, object? result)> SearchAsync(string? title, bool includeClosed);
    Task<(bool ok, int status, string? error, object? result)> GetByIdAsync(int id);
    Task<(bool ok, int status, string? error, object? result)> GetBidsAsync(int id);
    Task<(bool ok, int status, string? error, object? result)> UpdateAsync(int userId, int id, UpdateAuctionRequestDto dto);
}