namespace AuctionApi.DTOs.Auctions;

public class AuctionListItemDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public decimal StartingPrice { get; set; }
    public DateTime EndDate { get; set; }
    public string SellerName { get; set; } = "";
}