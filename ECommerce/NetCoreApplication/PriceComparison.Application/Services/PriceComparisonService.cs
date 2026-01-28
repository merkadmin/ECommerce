using AutoMapper;
using PriceComparison.Application.DTOs;
using PriceComparison.Application.Interfaces;
using PriceComparison.Core.Interfaces;

namespace PriceComparison.Application.Services;

public class PriceComparisonService : IPriceComparisonService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PriceComparisonService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PriceComparisonDto?> CompareProductPricesAsync(Guid productId)
    {
        var product = await _unitOfWork.Products.GetByIdWithPricesAsync(productId);
        if (product == null) return null;

        var prices = product.Prices.Where(p => p.IsAvailable).ToList();
        if (!prices.Any())
        {
            return new PriceComparisonDto
            {
                ProductId = productId,
                ProductName = product.Name,
                ProductImage = product.ImageUrl,
                PricesByRetailer = new List<RetailerPriceDto>()
            };
        }

        return new PriceComparisonDto
        {
            ProductId = productId,
            ProductName = product.Name,
            ProductImage = product.ImageUrl,
            LowestPrice = prices.Min(p => p.CurrentPrice),
            HighestPrice = prices.Max(p => p.CurrentPrice),
            AveragePrice = prices.Average(p => p.CurrentPrice),
            PricesByRetailer = prices
                .Select(p => new RetailerPriceDto
                {
                    RetailerId = p.RetailerId,
                    RetailerName = p.Retailer?.Name ?? string.Empty,
                    RetailerLogo = p.Retailer?.LogoUrl ?? string.Empty,
                    Price = p.CurrentPrice,
                    OriginalPrice = p.OriginalPrice,
                    DiscountPercent = p.DiscountPercent,
                    IsAvailable = p.IsAvailable,
                    ProductUrl = p.ProductUrl,
                    ShippingCost = p.ShippingCost,
                    LastUpdated = p.LastUpdated
                })
                .OrderBy(p => p.TotalPrice)
                .ToList()
        };
    }

    public async Task<decimal?> GetLowestPriceAsync(Guid productId)
    {
        return await _unitOfWork.Prices.GetLowestPriceForProductAsync(productId);
    }

    public async Task<PriceHistoryChartDto> GetPriceHistoryAsync(Guid productId, int days = 30)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(productId);
        if (product == null)
        {
            return new PriceHistoryChartDto();
        }

        var history = await _unitOfWork.PriceHistories.GetByProductIdAsync(productId, days);
        var retailers = await _unitOfWork.Retailers.GetActiveRetailersAsync();

        var colors = new[] { "#3B82F6", "#10B981", "#F59E0B", "#EF4444", "#8B5CF6", "#EC4899" };
        var colorIndex = 0;

        var series = history
            .GroupBy(h => h.RetailerId)
            .Select(g =>
            {
                var retailer = retailers.FirstOrDefault(r => r.Id == g.Key);
                return new PriceHistorySeriesDto
                {
                    RetailerId = g.Key,
                    RetailerName = retailer?.Name ?? "Unknown",
                    Color = colors[colorIndex++ % colors.Length],
                    DataPoints = g
                        .OrderBy(h => h.RecordedAt)
                        .Select(h => new PricePointDto
                        {
                            Date = h.RecordedAt,
                            Price = h.Price
                        })
                        .ToList()
                };
            })
            .ToList();

        var allPrices = history.Select(h => h.Price).ToList();

        return new PriceHistoryChartDto
        {
            ProductId = productId,
            ProductName = product.Name,
            Series = series,
            LowestEver = allPrices.Any() ? allPrices.Min() : 0,
            HighestEver = allPrices.Any() ? allPrices.Max() : 0,
            AveragePrice = allPrices.Any() ? allPrices.Average() : 0
        };
    }
}
