using Microsoft.EntityFrameworkCore;
using PriceComparison.Core.Entities;
using PriceComparison.Core.Interfaces;
using PriceComparison.Infrastructure.Data;

namespace PriceComparison.Infrastructure.Repositories;

public class PriceHistoryRepository : Repository<PriceHistory>, IPriceHistoryRepository
{
    public PriceHistoryRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<PriceHistory>> GetByProductIdAsync(Guid productId, int days = 30)
    {
        var startDate = DateTime.UtcNow.AddDays(-days);
        return await _dbSet
            .Include(ph => ph.Retailer)
            .Where(ph => ph.ProductId == productId && ph.RecordedAt >= startDate)
            .OrderBy(ph => ph.RecordedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<PriceHistory>> GetByProductAndRetailerAsync(Guid productId, Guid retailerId, int days = 30)
    {
        var startDate = DateTime.UtcNow.AddDays(-days);
        return await _dbSet
            .Include(ph => ph.Retailer)
            .Where(ph => ph.ProductId == productId &&
                        ph.RetailerId == retailerId &&
                        ph.RecordedAt >= startDate)
            .OrderBy(ph => ph.RecordedAt)
            .ToListAsync();
    }
}
