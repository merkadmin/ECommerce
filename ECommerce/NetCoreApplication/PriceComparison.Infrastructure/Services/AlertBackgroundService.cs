using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PriceComparison.Application.Interfaces;

namespace PriceComparison.Infrastructure.Services;

public class AlertBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AlertBackgroundService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(15);

    public AlertBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<AlertBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Alert Background Service is starting");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckAlertsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking price alerts");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInformation("Alert Background Service is stopping");
    }

    private async Task CheckAlertsAsync()
    {
        _logger.LogInformation("Checking price alerts...");

        using var scope = _serviceProvider.CreateScope();
        var alertService = scope.ServiceProvider.GetRequiredService<IAlertService>();

        await alertService.CheckAndTriggerAlertsAsync();

        _logger.LogInformation("Finished checking price alerts");
    }
}
