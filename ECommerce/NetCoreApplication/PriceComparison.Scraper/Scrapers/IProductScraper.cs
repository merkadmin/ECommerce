namespace PriceComparison.Scraper.Scrapers;

public interface IProductScraper
{
    string RetailerName { get; }
    Task<ScrapedProductPrice?> ScrapeProductPriceAsync(string productUrl);
    Task<List<ScrapedProductPrice>> SearchProductsAsync(string query);
}

public class ScrapedProductPrice
{
    public string ProductName { get; set; } = string.Empty;
    public string ProductUrl { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public decimal CurrentPrice { get; set; }
    public decimal? OriginalPrice { get; set; }
    public bool IsAvailable { get; set; }
    public decimal? ShippingCost { get; set; }
    public string? SKU { get; set; }
    public string? Brand { get; set; }
    public string RetailerName { get; set; } = string.Empty;
    public DateTime ScrapedAt { get; set; } = DateTime.UtcNow;
}
