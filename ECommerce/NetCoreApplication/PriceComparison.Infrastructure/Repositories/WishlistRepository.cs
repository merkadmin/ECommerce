using Microsoft.EntityFrameworkCore;
using PriceComparison.Core.Entities;
using PriceComparison.Core.Interfaces;
using PriceComparison.Infrastructure.Data;

namespace PriceComparison.Infrastructure.Repositories;

public class WishlistRepository : Repository<WishlistItem>, IWishlistRepository
{
    public WishlistRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<WishlistItem>> GetByUserIdAsync(Guid userId)
    {
        return await _dbSet
            .Include(w => w.Product)
            .Where(w => w.UserId == userId)
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync();
    }

    public async Task<WishlistItem?> GetByUserAndProductAsync(Guid userId, Guid productId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);
    }
}
