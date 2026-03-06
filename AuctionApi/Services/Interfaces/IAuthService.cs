namespace AuctionApi.Services.Interfaces;

public interface IAuthService
{
    Task<object> RegisterAsync(string name, string email, string password);
    Task<object> LoginAsync(string email, string password);
    Task ChangePasswordAsync(int userId, string currentPassword, string newPassword);
}