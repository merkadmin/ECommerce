using Microsoft.Extensions.Logging;

namespace PriceComparison.Scraper.Scrapers;

public class AmazonScraper : BaseScraper
{
    public override string RetailerName => "Amazon";

    public AmazonScraper(HttpClient httpClient, ILogger<AmazonScraper> logger)
        : base(httpClient, logger)
    {
    }

    public override async Task<ScrapedProductPrice?> ScrapeProductPriceAsync(string productUrl)
    {
        var doc = await GetHtmlDocumentAsync(productUrl);
        if (doc == null) return null;

        try
        {
            // Product title
            var titleNode = doc.DocumentNode.SelectSingleNode("//span[@id='productTitle']");
            var title = titleNode?.InnerText.Trim() ?? string.Empty;

            // Current price
            var priceNode = doc.DocumentNode.SelectSingleNode("//span[@class='a-price-whole']") ??
                           doc.DocumentNode.SelectSingleNode("//span[contains(@class, 'priceToPay')]//span[@class='a-price-whole']");
            var currentPrice = ParsePrice(priceNode?.InnerText);

            // Original price (if discounted)
            var originalPriceNode = doc.DocumentNode.SelectSingleNode("//span[@class='a-price a-text-price']//span[@class='a-offscreen']");
            var originalPrice = ParsePrice(originalPriceNode?.InnerText);

            // Availability
            var availabilityNode = doc.DocumentNode.SelectSingleNode("//div[@id='availability']//span");
            var isAvailable = ParseAvailability(availabilityNode?.InnerText);

            // Image
            var imageNode = doc.DocumentNode.SelectSingleNode("//img[@id='landingImage']");
            var imageUrl = imageNode?.GetAttributeValue("src", string.Empty) ?? string.Empty;

            // Brand
            var brandNode = doc.DocumentNode.SelectSingleNode("//a[@id='bylineInfo']");
            var brand = brandNode?.InnerText.Replace("Visit the", "").Replace("Store", "").Trim() ?? string.Empty;

            return new ScrapedProductPrice
            {
                ProductName = title,
                ProductUrl = productUrl,
                ImageUrl = imageUrl,
                CurrentPrice = currentPrice,
                OriginalPrice = originalPrice > currentPrice ? originalPrice : null,
                IsAvailable = isAvailable,
                Brand = brand,
                RetailerName = RetailerName
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing Amazon product page: {Url}", productUrl);
            return null;
        }
    }

    public override async Task<List<ScrapedProductPrice>> SearchProductsAsync(string query)
    {
        var products = new List<ScrapedProductPrice>();
        var searchUrl = $"https://www.amazon.com/s?k={Uri.EscapeDataString(query)}";

        var doc = await GetHtmlDocumentAsync(searchUrl);
        if (doc == null) return products;

        try
        {
            var productNodes = doc.DocumentNode.SelectNodes("//div[@data-component-type='s-search-result']");
            if (productNodes == null) return products;

            foreach (var node in productNodes.Take(20))
            {
                try
                {
                    var titleNode = node.SelectSingleNode(".//span[@class='a-size-medium a-color-base a-text-normal']") ??
                                   node.SelectSingleNode(".//span[@class='a-size-base-plus a-color-base a-text-normal']");
                    var title = titleNode?.InnerText.Trim() ?? string.Empty;

                    var linkNode = node.SelectSingleNode(".//a[@class='a-link-normal s-underline-text s-underline-link-text s-link-style a-text-normal']");
                    var productUrl = "https://www.amazon.com" + (linkNode?.GetAttributeValue("href", "") ?? "");

                    var priceNode = node.SelectSingleNode(".//span[@class='a-price-whole']");
                    var currentPrice = ParsePrice(priceNode?.InnerText);

                    var imageNode = node.SelectSingleNode(".//img[@class='s-image']");
                    var imageUrl = imageNode?.GetAttributeValue("src", "") ?? "";

                    if (!string.IsNullOrEmpty(title) && currentPrice > 0)
                    {
                        products.Add(new ScrapedProductPrice
                        {
                            ProductName = title,
                            ProductUrl = productUrl,
                            ImageUrl = imageUrl,
                            CurrentPrice = currentPrice,
                            IsAvailable = true,
                            RetailerName = RetailerName
                        });
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error parsing search result item");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing Amazon search results for query: {Query}", query);
        }

        return products;
    }
}
