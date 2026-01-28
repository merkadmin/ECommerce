using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

namespace PriceComparison.Scraper.Scrapers;

public abstract class BaseScraper : IProductScraper
{
    protected readonly HttpClient _httpClient;
    protected readonly ILogger _logger;

    public abstract string RetailerName { get; }

    protected BaseScraper(HttpClient httpClient, ILogger logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        // Configure HttpClient
        _httpClient.DefaultRequestHeaders.Add("User-Agent",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
        _httpClient.DefaultRequestHeaders.Add("Accept",
            "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8");
        _httpClient.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.5");
    }

    public abstract Task<ScrapedProductPrice?> ScrapeProductPriceAsync(string productUrl);
    public abstract Task<List<ScrapedProductPrice>> SearchProductsAsync(string query);

    protected async Task<HtmlDocument?> GetHtmlDocumentAsync(string url)
    {
        try
        {
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var html = await response.Content.ReadAsStringAsync();
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            return doc;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching URL: {Url}", url);
            return null;
        }
    }

    protected decimal ParsePrice(string? priceText)
    {
        if (string.IsNullOrWhiteSpace(priceText))
            return 0;

        // Remove currency symbols and whitespace
        var cleaned = new string(priceText.Where(c => char.IsDigit(c) || c == '.' || c == ',').ToArray());

        // Handle different decimal separators
        if (cleaned.Contains(',') && cleaned.Contains('.'))
        {
            // Assume format like 1,234.56
            cleaned = cleaned.Replace(",", "");
        }
        else if (cleaned.Contains(','))
        {
            // Could be 1234,56 (European) or 1,234 (thousands)
            var parts = cleaned.Split(',');
            if (parts.Length == 2 && parts[1].Length == 2)
            {
                // European format: 1234,56
                cleaned = cleaned.Replace(",", ".");
            }
            else
            {
                // Thousands separator: 1,234
                cleaned = cleaned.Replace(",", "");
            }
        }

        if (decimal.TryParse(cleaned, out var price))
            return price;

        return 0;
    }

    protected bool ParseAvailability(string? availabilityText)
    {
        if (string.IsNullOrWhiteSpace(availabilityText))
            return false;

        var lower = availabilityText.ToLower();
        return lower.Contains("in stock") ||
               lower.Contains("available") ||
               lower.Contains("add to cart") ||
               lower.Contains("buy now");
    }
}
