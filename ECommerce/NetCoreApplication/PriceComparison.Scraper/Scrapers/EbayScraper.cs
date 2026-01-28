using Microsoft.Extensions.Logging;

namespace PriceComparison.Scraper.Scrapers;

public class EbayScraper : BaseScraper
{
    public override string RetailerName => "eBay";

    public EbayScraper(HttpClient httpClient, ILogger<EbayScraper> logger)
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
            var titleNode = doc.DocumentNode.SelectSingleNode("//h1[@class='x-item-title__mainTitle']//span") ??
                           doc.DocumentNode.SelectSingleNode("//h1[contains(@class, 'it-ttl')]");
            var title = titleNode?.InnerText.Trim() ?? string.Empty;

            // Current price
            var priceNode = doc.DocumentNode.SelectSingleNode("//span[@class='ux-textspans']") ??
                           doc.DocumentNode.SelectSingleNode("//span[@id='prcIsum']");
            var currentPrice = ParsePrice(priceNode?.InnerText);

            // Image
            var imageNode = doc.DocumentNode.SelectSingleNode("//img[@id='icImg']") ??
                           doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'ux-image-carousel')]//img");
            var imageUrl = imageNode?.GetAttributeValue("src", string.Empty) ?? string.Empty;

            // Availability (most eBay listings are available if they show up)
            var isAvailable = currentPrice > 0;

            // Shipping cost
            var shippingNode = doc.DocumentNode.SelectSingleNode("//span[@id='fshippingCost']//span") ??
                              doc.DocumentNode.SelectSingleNode("//span[contains(@class, 'ux-textspans--BOLD')]");
            decimal? shippingCost = null;
            if (shippingNode != null)
            {
                var shippingText = shippingNode.InnerText.ToLower();
                if (shippingText.Contains("free"))
                {
                    shippingCost = 0;
                }
                else
                {
                    shippingCost = ParsePrice(shippingNode.InnerText);
                }
            }

            return new ScrapedProductPrice
            {
                ProductName = title,
                ProductUrl = productUrl,
                ImageUrl = imageUrl,
                CurrentPrice = currentPrice,
                IsAvailable = isAvailable,
                ShippingCost = shippingCost,
                RetailerName = RetailerName
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing eBay product page: {Url}", productUrl);
            return null;
        }
    }

    public override async Task<List<ScrapedProductPrice>> SearchProductsAsync(string query)
    {
        var products = new List<ScrapedProductPrice>();
        var searchUrl = $"https://www.ebay.com/sch/i.html?_nkw={Uri.EscapeDataString(query)}";

        var doc = await GetHtmlDocumentAsync(searchUrl);
        if (doc == null) return products;

        try
        {
            var productNodes = doc.DocumentNode.SelectNodes("//li[contains(@class, 's-item')]");
            if (productNodes == null) return products;

            foreach (var node in productNodes.Take(20))
            {
                try
                {
                    var titleNode = node.SelectSingleNode(".//div[@class='s-item__title']//span") ??
                                   node.SelectSingleNode(".//h3[@class='s-item__title']");
                    var title = titleNode?.InnerText.Trim() ?? string.Empty;

                    // Skip "Shop on eBay" promotional items
                    if (title.Contains("Shop on eBay", StringComparison.OrdinalIgnoreCase))
                        continue;

                    var linkNode = node.SelectSingleNode(".//a[@class='s-item__link']");
                    var productUrl = linkNode?.GetAttributeValue("href", "") ?? "";

                    var priceNode = node.SelectSingleNode(".//span[@class='s-item__price']");
                    var currentPrice = ParsePrice(priceNode?.InnerText);

                    var imageNode = node.SelectSingleNode(".//img[@class='s-item__image-img']");
                    var imageUrl = imageNode?.GetAttributeValue("src", "") ?? "";

                    if (!string.IsNullOrEmpty(title) && currentPrice > 0 && !string.IsNullOrEmpty(productUrl))
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
                    _logger.LogWarning(ex, "Error parsing eBay search result item");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing eBay search results for query: {Query}", query);
        }

        return products;
    }
}
