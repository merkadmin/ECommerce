namespace PriceComparison.Application.DTOs;

public class PriceComparisonDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductImage { get; set; } = string.Empty;
    public decimal LowestPrice { get; set; }
    public decimal HighestPrice { get; set; }
    public decimal AveragePrice { get; set; }
    public decimal PriceDifference => HighestPrice - LowestPrice;
    public decimal SavingsPercent => HighestPrice > 0 ? ((HighestPrice - LowestPrice) / HighestPrice) * 100 : 0;
    public List<RetailerPriceDto> PricesByRetailer { get; set; } = new();
    public RetailerPriceDto? BestDeal => PricesByRetailer.Where(p => p.IsAvailable).MinBy(p => p.TotalPrice);
}
