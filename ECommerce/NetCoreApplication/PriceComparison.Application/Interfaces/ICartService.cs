using PriceComparison.Application.DTOs;

namespace PriceComparison.Application.Interfaces;

public interface ICartService
{
    Task<CartSummaryDto> GetUserCartAsync(Guid userId);
    Task<CartItemDto> AddToCartAsync(Guid userId, AddToCartDto dto);
    Task<CartItemDto?> UpdateCartItemAsync(Guid id, UpdateCartItemDto dto);
    Task<bool> RemoveFromCartAsync(Guid id);
    Task ClearCartAsync(Guid userId);
}
