using Microsoft.EntityFrameworkCore;
using PriceComparison.Core.Entities;
using PriceComparison.Core.Interfaces;
using PriceComparison.Infrastructure.Data;

namespace PriceComparison.Infrastructure.Repositories;

public class PriceAlertRepository : Repository<PriceAlert>, IPriceAlertRepository
{
    public PriceAlertRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<PriceAlert>> GetByUserIdAsync(Guid userId)
    {
        return await _dbSet
            .Include(pa => pa.Product)
            .Where(pa => pa.UserId == userId)
            .OrderByDescending(pa => pa.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<PriceAlert>> GetActiveAlertsAsync()
    {
        return await _dbSet
            .Include(pa => pa.User)
            .Include(pa => pa.Product)
            .Where(pa => pa.IsActive && !pa.IsTriggered)
            .ToListAsync();
    }

    public async Task<PriceAlert?> GetByUserAndProductAsync(Guid userId, Guid productId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(pa => pa.UserId == userId && pa.ProductId == productId);
    }
}
