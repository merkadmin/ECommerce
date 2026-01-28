using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PriceComparison.Scraper.Scrapers;

namespace PriceComparison.Scraper.Jobs;

public class ProductSearchJob
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ProductSearchJob> _logger;

    public ProductSearchJob(
        IServiceProvider serviceProvider,
        ILogger<ProductSearchJob> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task<List<ScrapedProductPrice>> SearchAcrossRetailersAsync(string query, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Searching for '{Query}' across all retailers", query);

        var allResults = new List<ScrapedProductPrice>();

        using var scope = _serviceProvider.CreateScope();
        var httpClient = new HttpClient();

        var scrapers = new List<IProductScraper>
        {
            new AmazonScraper(httpClient, scope.ServiceProvider.GetRequiredService<ILogger<AmazonScraper>>()),
            new EbayScraper(httpClient, scope.ServiceProvider.GetRequiredService<ILogger<EbayScraper>>())
        };

        var tasks = scrapers.Select(async scraper =>
        {
            try
            {
                var results = await scraper.SearchProductsAsync(query);
                return results;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error searching {Retailer}", scraper.RetailerName);
                return new List<ScrapedProductPrice>();
            }
        });

        var results = await Task.WhenAll(tasks);
        allResults.AddRange(results.SelectMany(r => r));

        _logger.LogInformation("Found {Count} products across all retailers for '{Query}'",
            allResults.Count, query);

        return allResults;
    }

    public async Task<Dictionary<string, ScrapedProductPrice?>> GetPricesForProductAsync(
        string productName,
        CancellationToken cancellationToken = default)
    {
        var results = new Dictionary<string, ScrapedProductPrice?>();

        using var scope = _serviceProvider.CreateScope();
        var httpClient = new HttpClient();

        var scrapers = new List<IProductScraper>
        {
            new AmazonScraper(httpClient, scope.ServiceProvider.GetRequiredService<ILogger<AmazonScraper>>()),
            new EbayScraper(httpClient, scope.ServiceProvider.GetRequiredService<ILogger<EbayScraper>>())
        };

        foreach (var scraper in scrapers)
        {
            try
            {
                var searchResults = await scraper.SearchProductsAsync(productName);
                var bestMatch = searchResults.FirstOrDefault();
                results[scraper.RetailerName] = bestMatch;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error getting price from {Retailer}", scraper.RetailerName);
                results[scraper.RetailerName] = null;
            }

            // Delay between retailers
            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
        }

        return results;
    }
}
