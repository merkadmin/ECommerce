using PriceComparison.Application.DTOs;

namespace PriceComparison.Application.Interfaces;

public interface IPriceService
{
    Task<List<PriceDto>> GetByProductIdAsync(Guid productId);
    Task<PriceDto?> GetByIdAsync(Guid id);
    Task<PriceDto> CreateAsync(CreatePriceDto dto);
    Task<PriceDto?> UpdateAsync(Guid id, UpdatePriceDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task UpdatePriceAsync(Guid productId, Guid retailerId, decimal newPrice, decimal? originalPrice, bool isAvailable, decimal? shippingCost);
}
