namespace PriceComparison.Application.DTOs;

public class PriceDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid RetailerId { get; set; }
    public string RetailerName { get; set; } = string.Empty;
    public decimal CurrentPrice { get; set; }
    public decimal? OriginalPrice { get; set; }
    public decimal? DiscountPercent { get; set; }
    public string ProductUrl { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
    public decimal? ShippingCost { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class RetailerPriceDto
{
    public Guid RetailerId { get; set; }
    public string RetailerName { get; set; } = string.Empty;
    public string RetailerLogo { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? OriginalPrice { get; set; }
    public decimal? DiscountPercent { get; set; }
    public bool IsAvailable { get; set; }
    public string ProductUrl { get; set; } = string.Empty;
    public decimal? ShippingCost { get; set; }
    public decimal TotalPrice => Price + (ShippingCost ?? 0);
    public DateTime LastUpdated { get; set; }
}

public class CreatePriceDto
{
    public Guid ProductId { get; set; }
    public Guid RetailerId { get; set; }
    public decimal CurrentPrice { get; set; }
    public decimal? OriginalPrice { get; set; }
    public string ProductUrl { get; set; } = string.Empty;
    public bool IsAvailable { get; set; } = true;
    public decimal? ShippingCost { get; set; }
}

public class UpdatePriceDto
{
    public decimal CurrentPrice { get; set; }
    public decimal? OriginalPrice { get; set; }
    public bool IsAvailable { get; set; }
    public decimal? ShippingCost { get; set; }
}
