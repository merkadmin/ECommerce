using Microsoft.EntityFrameworkCore;
using PriceComparison.Core.Entities;
using PriceComparison.Core.Interfaces;
using PriceComparison.Infrastructure.Data;

namespace PriceComparison.Infrastructure.Repositories;

public class PriceRepository : Repository<Price>, IPriceRepository
{
    public PriceRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Price>> GetByProductIdAsync(Guid productId)
    {
        return await _dbSet
            .Include(p => p.Retailer)
            .Where(p => p.ProductId == productId)
            .OrderBy(p => p.CurrentPrice)
            .ToListAsync();
    }

    public async Task<Price?> GetByProductAndRetailerAsync(Guid productId, Guid retailerId)
    {
        return await _dbSet
            .Include(p => p.Retailer)
            .FirstOrDefaultAsync(p => p.ProductId == productId && p.RetailerId == retailerId);
    }

    public async Task<decimal?> GetLowestPriceForProductAsync(Guid productId)
    {
        var prices = await _dbSet
            .Where(p => p.ProductId == productId && p.IsAvailable)
            .Select(p => p.CurrentPrice)
            .ToListAsync();

        return prices.Any() ? prices.Min() : null;
    }

    public async Task<IEnumerable<Price>> GetAvailablePricesForProductAsync(Guid productId)
    {
        return await _dbSet
            .Include(p => p.Retailer)
            .Where(p => p.ProductId == productId && p.IsAvailable)
            .OrderBy(p => p.CurrentPrice)
            .ToListAsync();
    }
}
