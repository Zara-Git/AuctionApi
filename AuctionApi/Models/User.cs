using System.Security.Cryptography;

namespace AuctionApi.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public bool IsActive { get; set; } = true;
    public bool IsAdmin { get; set; } = false;
    

    public List<Auction> Auctions { get; set; } = new();
    public List<Bid> Bids { get; set; } = new();
}