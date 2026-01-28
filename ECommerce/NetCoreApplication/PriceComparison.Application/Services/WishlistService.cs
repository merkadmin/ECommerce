using AutoMapper;
using PriceComparison.Application.DTOs;
using PriceComparison.Application.Interfaces;
using PriceComparison.Core.Entities;
using PriceComparison.Core.Interfaces;

namespace PriceComparison.Application.Services;

public class WishlistService : IWishlistService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPriceComparisonService _priceComparisonService;

    public WishlistService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IPriceComparisonService priceComparisonService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _priceComparisonService = priceComparisonService;
    }

    public async Task<List<WishlistItemDto>> GetUserWishlistAsync(Guid userId)
    {
        var items = await _unitOfWork.Wishlists.GetByUserIdAsync(userId);
        var dtos = new List<WishlistItemDto>();

        foreach (var item in items)
        {
            var dto = _mapper.Map<WishlistItemDto>(item);
            dto.LowestPrice = await _priceComparisonService.GetLowestPriceAsync(item.ProductId);
            dtos.Add(dto);
        }

        return dtos;
    }

    public async Task<WishlistItemDto> AddToWishlistAsync(Guid userId, AddToWishlistDto dto)
    {
        var existing = await _unitOfWork.Wishlists.GetByUserAndProductAsync(userId, dto.ProductId);
        if (existing != null)
        {
            return _mapper.Map<WishlistItemDto>(existing);
        }

        var item = new WishlistItem
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ProductId = dto.ProductId,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Wishlists.AddAsync(item);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<WishlistItemDto>(item);
    }

    public async Task<WishlistItemDto?> UpdateAsync(Guid id, UpdateWishlistItemDto dto)
    {
        var item = await _unitOfWork.Wishlists.GetByIdAsync(id);
        if (item == null) return null;

        item.Notes = dto.Notes;
        item.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Wishlists.Update(item);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<WishlistItemDto>(item);
    }

    public async Task<bool> RemoveFromWishlistAsync(Guid id)
    {
        var item = await _unitOfWork.Wishlists.GetByIdAsync(id);
        if (item == null) return false;

        _unitOfWork.Wishlists.Remove(item);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> IsInWishlistAsync(Guid userId, Guid productId)
    {
        var item = await _unitOfWork.Wishlists.GetByUserAndProductAsync(userId, productId);
        return item != null;
    }
}
