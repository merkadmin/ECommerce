using PriceComparison.Application.DTOs;

namespace PriceComparison.Application.Interfaces;

public interface IWishlistService
{
    Task<List<WishlistItemDto>> GetUserWishlistAsync(Guid userId);
    Task<WishlistItemDto> AddToWishlistAsync(Guid userId, AddToWishlistDto dto);
    Task<WishlistItemDto?> UpdateAsync(Guid id, UpdateWishlistItemDto dto);
    Task<bool> RemoveFromWishlistAsync(Guid id);
    Task<bool> IsInWishlistAsync(Guid userId, Guid productId);
}
