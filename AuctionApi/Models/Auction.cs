namespace AuctionApi.Models;

public class Auction
{
    public int Id { get; set; }

    public string Title { get; set; } = "";
    public string Description { get; set; } = "";

    public decimal StartingPrice { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    // FK + navigation
    public int CreatedByUserId { get; set; }
    public User? CreatedByUser { get; set; }

    public bool IsDisabled { get; set; } = false;

    public List<Bid> Bids { get; set; } = new();
}