using PriceComparison.Core.Entities;

namespace PriceComparison.Core.Interfaces;

public interface ICartRepository : IRepository<CartItem>
{
    Task<IEnumerable<CartItem>> GetByUserIdAsync(Guid userId);
    Task<CartItem?> GetByUserAndProductAsync(Guid userId, Guid productId, Guid? retailerId);
    Task ClearCartAsync(Guid userId);
}
