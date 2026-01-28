using Microsoft.EntityFrameworkCore;
using PriceComparison.Core.Entities;
using PriceComparison.Core.Interfaces;
using PriceComparison.Infrastructure.Data;

namespace PriceComparison.Infrastructure.Repositories;

public class RetailerRepository : Repository<Retailer>, IRetailerRepository
{
    public RetailerRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Retailer>> GetActiveRetailersAsync()
    {
        return await _dbSet
            .Where(r => r.IsActive)
            .OrderBy(r => r.Name)
            .ToListAsync();
    }
}
