using AutoMapper;
using PriceComparison.Application.DTOs;
using PriceComparison.Application.Interfaces;
using PriceComparison.Core.Entities;
using PriceComparison.Core.Interfaces;

namespace PriceComparison.Application.Services;

public class PriceService : IPriceService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PriceService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<PriceDto>> GetByProductIdAsync(Guid productId)
    {
        var prices = await _unitOfWork.Prices.GetByProductIdAsync(productId);
        return _mapper.Map<List<PriceDto>>(prices);
    }

    public async Task<PriceDto?> GetByIdAsync(Guid id)
    {
        var price = await _unitOfWork.Prices.GetByIdAsync(id);
        return price == null ? null : _mapper.Map<PriceDto>(price);
    }

    public async Task<PriceDto> CreateAsync(CreatePriceDto dto)
    {
        var price = _mapper.Map<Price>(dto);
        price.Id = Guid.NewGuid();
        price.CreatedAt = DateTime.UtcNow;
        price.UpdatedAt = DateTime.UtcNow;
        price.LastUpdated = DateTime.UtcNow;

        if (dto.OriginalPrice.HasValue && dto.OriginalPrice > dto.CurrentPrice)
        {
            price.DiscountPercent = ((dto.OriginalPrice.Value - dto.CurrentPrice) / dto.OriginalPrice.Value) * 100;
        }

        await _unitOfWork.Prices.AddAsync(price);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<PriceDto>(price);
    }

    public async Task<PriceDto?> UpdateAsync(Guid id, UpdatePriceDto dto)
    {
        var price = await _unitOfWork.Prices.GetByIdAsync(id);
        if (price == null) return null;

        price.CurrentPrice = dto.CurrentPrice;
        price.OriginalPrice = dto.OriginalPrice;
        price.IsAvailable = dto.IsAvailable;
        price.ShippingCost = dto.ShippingCost;
        price.UpdatedAt = DateTime.UtcNow;
        price.LastUpdated = DateTime.UtcNow;

        if (dto.OriginalPrice.HasValue && dto.OriginalPrice > dto.CurrentPrice)
        {
            price.DiscountPercent = ((dto.OriginalPrice.Value - dto.CurrentPrice) / dto.OriginalPrice.Value) * 100;
        }

        _unitOfWork.Prices.Update(price);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<PriceDto>(price);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var price = await _unitOfWork.Prices.GetByIdAsync(id);
        if (price == null) return false;

        _unitOfWork.Prices.Remove(price);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task UpdatePriceAsync(Guid productId, Guid retailerId, decimal newPrice, decimal? originalPrice, bool isAvailable, decimal? shippingCost)
    {
        var existingPrice = await _unitOfWork.Prices.GetByProductAndRetailerAsync(productId, retailerId);

        if (existingPrice != null)
        {
            // Record history if price changed
            if (existingPrice.CurrentPrice != newPrice)
            {
                var history = new PriceHistory
                {
                    Id = Guid.NewGuid(),
                    ProductId = productId,
                    RetailerId = retailerId,
                    Price = existingPrice.CurrentPrice,
                    RecordedAt = existingPrice.LastUpdated
                };
                await _unitOfWork.PriceHistories.AddAsync(history);
            }

            existingPrice.CurrentPrice = newPrice;
            existingPrice.OriginalPrice = originalPrice;
            existingPrice.IsAvailable = isAvailable;
            existingPrice.ShippingCost = shippingCost;
            existingPrice.LastUpdated = DateTime.UtcNow;
            existingPrice.UpdatedAt = DateTime.UtcNow;

            if (originalPrice.HasValue && originalPrice > newPrice)
            {
                existingPrice.DiscountPercent = ((originalPrice.Value - newPrice) / originalPrice.Value) * 100;
            }

            _unitOfWork.Prices.Update(existingPrice);
        }

        await _unitOfWork.SaveChangesAsync();
    }
}
