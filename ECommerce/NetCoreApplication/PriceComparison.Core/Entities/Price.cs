namespace PriceComparison.Core.Entities;

public class Price : BaseEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public Guid RetailerId { get; set; }
    public Retailer Retailer { get; set; } = null!;
    public decimal CurrentPrice { get; set; }
    public decimal? OriginalPrice { get; set; }
    public decimal? DiscountPercent { get; set; }
    public string ProductUrl { get; set; } = string.Empty;
    public bool IsAvailable { get; set; } = true;
    public decimal? ShippingCost { get; set; }
    public DateTime LastUpdated { get; set; }
}
