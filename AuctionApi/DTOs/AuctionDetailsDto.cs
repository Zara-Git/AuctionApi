namespace AuctionApi.DTOs.Auctions;

public class AuctionDetailsDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal StartingPrice { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int CreatedByUserId { get; set; }
    public string SellerName { get; set; } = "";
    public bool IsOpen { get; set; }
    public decimal? HighestBid { get; set; }
}