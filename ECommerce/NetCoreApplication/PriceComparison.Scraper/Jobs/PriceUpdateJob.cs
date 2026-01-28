using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PriceComparison.Application.Interfaces;
using PriceComparison.Core.Interfaces;
using PriceComparison.Scraper.Scrapers;

namespace PriceComparison.Scraper.Jobs;

public class PriceUpdateJob : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PriceUpdateJob> _logger;
    private readonly TimeSpan _updateInterval = TimeSpan.FromHours(6);

    public PriceUpdateJob(
        IServiceProvider serviceProvider,
        ILogger<PriceUpdateJob> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Price Update Job is starting");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await UpdateAllPricesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating prices");
            }

            await Task.Delay(_updateInterval, stoppingToken);
        }

        _logger.LogInformation("Price Update Job is stopping");
    }

    private async Task UpdateAllPricesAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting price update cycle");

        using var scope = _serviceProvider.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var priceService = scope.ServiceProvider.GetRequiredService<IPriceService>();

        // Get all prices that need updating
        var prices = await unitOfWork.Prices.GetAllAsync();

        var httpClient = new HttpClient();
        var scrapers = new Dictionary<string, IProductScraper>
        {
            { "Amazon", new AmazonScraper(httpClient, scope.ServiceProvider.GetRequiredService<ILogger<AmazonScraper>>()) },
            { "eBay", new EbayScraper(httpClient, scope.ServiceProvider.GetRequiredService<ILogger<EbayScraper>>()) }
        };

        var updatedCount = 0;
        var errorCount = 0;

        foreach (var price in prices)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            try
            {
                var retailer = await unitOfWork.Retailers.GetByIdAsync(price.RetailerId);
                if (retailer == null || !retailer.IsActive)
                    continue;

                IProductScraper scraper;
                if (scrapers.TryGetValue(retailer.Name, out var knownScraper))
                {
                    scraper = knownScraper;
                }
                else
                {
                    scraper = new GenericScraper(
                        httpClient,
                        scope.ServiceProvider.GetRequiredService<ILogger<GenericScraper>>(),
                        retailer.Name);
                }

                var scrapedPrice = await scraper.ScrapeProductPriceAsync(price.ProductUrl);
                if (scrapedPrice != null && scrapedPrice.CurrentPrice > 0)
                {
                    await priceService.UpdatePriceAsync(
                        price.ProductId,
                        price.RetailerId,
                        scrapedPrice.CurrentPrice,
                        scrapedPrice.OriginalPrice,
                        scrapedPrice.IsAvailable,
                        scrapedPrice.ShippingCost);

                    updatedCount++;
                    _logger.LogDebug("Updated price for product {ProductId} from {Retailer}: {Price}",
                        price.ProductId, retailer.Name, scrapedPrice.CurrentPrice);
                }

                // Add delay to avoid rate limiting
                await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
            }
            catch (Exception ex)
            {
                errorCount++;
                _logger.LogWarning(ex, "Error updating price for product {ProductId}", price.ProductId);
            }
        }

        _logger.LogInformation("Price update cycle completed. Updated: {Updated}, Errors: {Errors}",
            updatedCount, errorCount);
    }
}
