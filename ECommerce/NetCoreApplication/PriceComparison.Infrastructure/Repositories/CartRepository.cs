using Microsoft.EntityFrameworkCore;
using PriceComparison.Core.Entities;
using PriceComparison.Core.Interfaces;
using PriceComparison.Infrastructure.Data;

namespace PriceComparison.Infrastructure.Repositories;

public class CartRepository : Repository<CartItem>, ICartRepository
{
    public CartRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<CartItem>> GetByUserIdAsync(Guid userId)
    {
        return await _dbSet
            .Include(c => c.Product)
            .Include(c => c.Retailer)
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<CartItem?> GetByUserAndProductAsync(Guid userId, Guid productId, Guid? retailerId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => c.UserId == userId &&
                                     c.ProductId == productId &&
                                     c.RetailerId == retailerId);
    }

    public async Task ClearCartAsync(Guid userId)
    {
        var items = await _dbSet.Where(c => c.UserId == userId).ToListAsync();
        _dbSet.RemoveRange(items);
    }
}
