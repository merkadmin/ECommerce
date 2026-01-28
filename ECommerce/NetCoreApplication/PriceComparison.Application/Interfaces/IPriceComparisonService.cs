using PriceComparison.Application.DTOs;

namespace PriceComparison.Application.Interfaces;

public interface IPriceComparisonService
{
    Task<PriceComparisonDto?> CompareProductPricesAsync(Guid productId);
    Task<decimal?> GetLowestPriceAsync(Guid productId);
    Task<PriceHistoryChartDto> GetPriceHistoryAsync(Guid productId, int days = 30);
}
