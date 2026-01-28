namespace PriceComparison.Core.Entities;

public class PriceHistory
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public Guid RetailerId { get; set; }
    public Retailer Retailer { get; set; } = null!;
    public decimal Price { get; set; }
    public DateTime RecordedAt { get; set; }
}
