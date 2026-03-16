using AuctionApi.Models;
using AuctionApi.Repositories;
using AuctionApi.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuctionApi.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _repo;
    private readonly IConfiguration _config;

    public AuthService(IAuthRepository repo, IConfiguration config)
    {
        _repo = repo;
        _config = config;
    }

    public async Task<object> RegisterAsync(string name, string email, string password)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.");

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.");

        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password is required.");

        email = email.Trim().ToLower();

        var exists = await _repo.EmailExistsAsync(email);
        if (exists)
            throw new ArgumentException("Email already exists.");

        var user = new User
        {
            Name = name.Trim(),
            Email = email,
            PasswordHash = Hash(password),
            IsActive = true
        };

        await _repo.AddUserAsync(user);
        await _repo.SaveChangesAsync();

        return new
        {
            user.Id,
            user.Name,
            user.Email
        };
    }

    public async Task<object> LoginAsync(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.");

        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password is required.");

        email = email.Trim().ToLower();

        var user = await _repo.GetByEmailAsync(email);
        if (user == null)
            throw new UnauthorizedAccessException("Invalid email or password.");

        if (!user.IsActive)
            throw new UnauthorizedAccessException("Account is disabled.");

        var hash = Hash(password);
        if (user.PasswordHash != hash)
            throw new UnauthorizedAccessException("Invalid email or password.");

        var token = CreateToken(user);

        return new
        {
            user.Id,
            user.Name,
            user.Email,
            token
        };
    }

    public async Task ChangePasswordAsync(int userId, string currentPassword, string newPassword)
    {
        if (string.IsNullOrWhiteSpace(currentPassword))
            throw new ArgumentException("CurrentPassword is required.");

        if (string.IsNullOrWhiteSpace(newPassword))
            throw new ArgumentException("NewPassword is required.");

        if (newPassword.Length < 6)
            throw new ArgumentException("New password must be at least 6 characters.");

        var user = await _repo.GetByIdAsync(userId);
        if (user == null)
            throw new Exception("User not found.");

        if (!user.IsActive)
            throw new UnauthorizedAccessException("Account is disabled.");

        var currentHash = Hash(currentPassword);
        if (user.PasswordHash != currentHash)
            throw new UnauthorizedAccessException("Current password is incorrect.");

        user.PasswordHash = Hash(newPassword);
        await _repo.SaveChangesAsync();
    }

    private static string Hash(string input)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes);
    }

    private string CreateToken(User user)
    {
        var key = _config["Jwt:Key"]!;
        var issuer = _config["Jwt:Issuer"]!;
        var audience = _config["Jwt:Audience"]!;
        var expiresMinutes = int.Parse(_config["Jwt:ExpiresMinutes"] ?? "120");
        var claims = new List<Claim>
{
    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
    new Claim(ClaimTypes.Name, user.Name),
    new Claim(ClaimTypes.Email, user.Email),
    new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User")
}; ;

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}