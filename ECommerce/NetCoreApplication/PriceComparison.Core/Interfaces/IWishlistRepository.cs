using PriceComparison.Core.Entities;

namespace PriceComparison.Core.Interfaces;

public interface IWishlistRepository : IRepository<WishlistItem>
{
    Task<IEnumerable<WishlistItem>> GetByUserIdAsync(Guid userId);
    Task<WishlistItem?> GetByUserAndProductAsync(Guid userId, Guid productId);
}
