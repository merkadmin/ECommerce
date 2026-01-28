using Microsoft.Extensions.Logging;

namespace PriceComparison.Scraper.Scrapers;

public class GenericScraper : BaseScraper
{
    private readonly string _retailerName;

    public override string RetailerName => _retailerName;

    public GenericScraper(HttpClient httpClient, ILogger<GenericScraper> logger, string retailerName)
        : base(httpClient, logger)
    {
        _retailerName = retailerName;
    }

    public override async Task<ScrapedProductPrice?> ScrapeProductPriceAsync(string productUrl)
    {
        var doc = await GetHtmlDocumentAsync(productUrl);
        if (doc == null) return null;

        try
        {
            // Try common selectors for product title
            var titleSelectors = new[]
            {
                "//h1[contains(@class, 'product') and contains(@class, 'title')]",
                "//h1[contains(@class, 'product-title')]",
                "//h1[contains(@class, 'pdp-title')]",
                "//h1[@itemprop='name']",
                "//meta[@property='og:title']/@content",
                "//title"
            };

            string title = string.Empty;
            foreach (var selector in titleSelectors)
            {
                var node = doc.DocumentNode.SelectSingleNode(selector);
                if (node != null)
                {
                    title = selector.Contains("@content") || selector.Contains("@name")
                        ? node.GetAttributeValue("content", "") ?? node.GetAttributeValue("name", "")
                        : node.InnerText.Trim();
                    if (!string.IsNullOrEmpty(title)) break;
                }
            }

            // Try common selectors for price
            var priceSelectors = new[]
            {
                "//span[contains(@class, 'price')]",
                "//div[contains(@class, 'price')]//span",
                "//meta[@property='product:price:amount']/@content",
                "//span[@itemprop='price']",
                "//*[contains(@class, 'current-price')]",
                "//*[contains(@class, 'sale-price')]"
            };

            decimal currentPrice = 0;
            foreach (var selector in priceSelectors)
            {
                var node = doc.DocumentNode.SelectSingleNode(selector);
                if (node != null)
                {
                    var priceText = selector.Contains("@content")
                        ? node.GetAttributeValue("content", "")
                        : node.InnerText;
                    currentPrice = ParsePrice(priceText);
                    if (currentPrice > 0) break;
                }
            }

            // Try common selectors for image
            var imageSelectors = new[]
            {
                "//img[contains(@class, 'product')]/@src",
                "//meta[@property='og:image']/@content",
                "//img[@itemprop='image']/@src",
                "//div[contains(@class, 'product-image')]//img/@src"
            };

            string imageUrl = string.Empty;
            foreach (var selector in imageSelectors)
            {
                var node = doc.DocumentNode.SelectSingleNode(selector);
                if (node != null)
                {
                    imageUrl = node.GetAttributeValue("src", "") ?? node.GetAttributeValue("content", "");
                    if (!string.IsNullOrEmpty(imageUrl)) break;
                }
            }

            // Make image URL absolute if needed
            if (!string.IsNullOrEmpty(imageUrl) && imageUrl.StartsWith("//"))
            {
                imageUrl = "https:" + imageUrl;
            }
            else if (!string.IsNullOrEmpty(imageUrl) && imageUrl.StartsWith("/"))
            {
                var uri = new Uri(productUrl);
                imageUrl = $"{uri.Scheme}://{uri.Host}{imageUrl}";
            }

            return new ScrapedProductPrice
            {
                ProductName = title,
                ProductUrl = productUrl,
                ImageUrl = imageUrl,
                CurrentPrice = currentPrice,
                IsAvailable = currentPrice > 0,
                RetailerName = RetailerName
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing product page with generic scraper: {Url}", productUrl);
            return null;
        }
    }

    public override Task<List<ScrapedProductPrice>> SearchProductsAsync(string query)
    {
        // Generic scraper doesn't support search - would need retailer-specific implementation
        _logger.LogWarning("Search not supported for generic scraper");
        return Task.FromResult(new List<ScrapedProductPrice>());
    }
}
