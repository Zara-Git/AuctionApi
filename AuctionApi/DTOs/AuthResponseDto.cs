namespace AuctionApi.DTOs.Auth;

public class AuthResponseDto
{
    public int UserId { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
}
