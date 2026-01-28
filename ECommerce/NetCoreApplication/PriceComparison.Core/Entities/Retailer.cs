namespace PriceComparison.Core.Entities;

public class Retailer : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    public decimal? AverageRating { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<Price> Prices { get; set; } = new List<Price>();
    public ICollection<PriceHistory> PriceHistories { get; set; } = new List<PriceHistory>();
}
