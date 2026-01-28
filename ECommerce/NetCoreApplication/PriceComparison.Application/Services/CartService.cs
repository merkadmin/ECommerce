using AutoMapper;
using PriceComparison.Application.DTOs;
using PriceComparison.Application.Interfaces;
using PriceComparison.Core.Entities;
using PriceComparison.Core.Interfaces;

namespace PriceComparison.Application.Services;

public class CartService : ICartService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CartService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CartSummaryDto> GetUserCartAsync(Guid userId)
    {
        var items = await _unitOfWork.Carts.GetByUserIdAsync(userId);
        var cartItems = new List<CartItemDto>();

        foreach (var item in items)
        {
            var dto = _mapper.Map<CartItemDto>(item);

            // Get price from selected retailer
            if (item.RetailerId.HasValue)
            {
                var price = await _unitOfWork.Prices.GetByProductAndRetailerAsync(item.ProductId, item.RetailerId.Value);
                if (price != null)
                {
                    dto.Price = price.CurrentPrice;
                    dto.ShippingCost = price.ShippingCost;
                }
            }
            else
            {
                // Get lowest price if no retailer selected
                var lowestPrice = await _unitOfWork.Prices.GetLowestPriceForProductAsync(item.ProductId);
                dto.Price = lowestPrice ?? 0;
            }

            cartItems.Add(dto);
        }

        return new CartSummaryDto
        {
            Items = cartItems
        };
    }

    public async Task<CartItemDto> AddToCartAsync(Guid userId, AddToCartDto dto)
    {
        var existing = await _unitOfWork.Carts.GetByUserAndProductAsync(userId, dto.ProductId, dto.RetailerId);
        if (existing != null)
        {
            existing.Quantity += dto.Quantity;
            existing.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Carts.Update(existing);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<CartItemDto>(existing);
        }

        var item = new CartItem
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ProductId = dto.ProductId,
            RetailerId = dto.RetailerId,
            Quantity = dto.Quantity,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Carts.AddAsync(item);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<CartItemDto>(item);
    }

    public async Task<CartItemDto?> UpdateCartItemAsync(Guid id, UpdateCartItemDto dto)
    {
        var item = await _unitOfWork.Carts.GetByIdAsync(id);
        if (item == null) return null;

        if (dto.Quantity <= 0)
        {
            _unitOfWork.Carts.Remove(item);
            await _unitOfWork.SaveChangesAsync();
            return null;
        }

        item.Quantity = dto.Quantity;
        item.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Carts.Update(item);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<CartItemDto>(item);
    }

    public async Task<bool> RemoveFromCartAsync(Guid id)
    {
        var item = await _unitOfWork.Carts.GetByIdAsync(id);
        if (item == null) return false;

        _unitOfWork.Carts.Remove(item);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task ClearCartAsync(Guid userId)
    {
        await _unitOfWork.Carts.ClearCartAsync(userId);
        await _unitOfWork.SaveChangesAsync();
    }
}
