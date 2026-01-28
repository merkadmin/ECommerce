using Microsoft.EntityFrameworkCore;
using PriceComparison.Core.Entities;
using PriceComparison.Core.Interfaces;
using PriceComparison.Infrastructure.Data;

namespace PriceComparison.Infrastructure.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Product?> GetByIdWithPricesAsync(Guid id)
    {
        return await _dbSet
            .Include(p => p.Prices)
                .ThenInclude(pr => pr.Retailer)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Product?> GetByIdWithDetailsAsync(Guid id)
    {
        return await _dbSet
            .Include(p => p.Category)
            .Include(p => p.Prices.Where(pr => pr.IsAvailable))
                .ThenInclude(pr => pr.Retailer)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(Guid categoryId)
    {
        return await _dbSet
            .Include(p => p.Category)
            .Include(p => p.Prices)
            .Where(p => p.CategoryId == categoryId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> SearchAsync(string searchTerm)
    {
        var term = searchTerm.ToLower();
        return await _dbSet
            .Include(p => p.Category)
            .Include(p => p.Prices)
            .Where(p => p.Name.ToLower().Contains(term) ||
                       p.Description.ToLower().Contains(term) ||
                       p.Brand.ToLower().Contains(term) ||
                       p.SKU.ToLower().Contains(term))
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetTrendingAsync(int count = 10)
    {
        return await _dbSet
            .Include(p => p.Category)
            .Include(p => p.Prices)
            .OrderByDescending(p => p.Prices.Count)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetDealsAsync(int count = 10)
    {
        return await _dbSet
            .Include(p => p.Category)
            .Include(p => p.Prices)
            .Where(p => p.Prices.Any(pr => pr.IsAvailable && pr.DiscountPercent > 0))
            .OrderByDescending(p => p.Prices.Where(pr => pr.IsAvailable).Max(pr => pr.DiscountPercent))
            .Take(count)
            .ToListAsync();
    }

    public async Task<(IEnumerable<Product> Products, int TotalCount)> GetPagedAsync(
        string? search,
        Guid? categoryId,
        decimal? minPrice,
        decimal? maxPrice,
        List<Guid>? retailerIds,
        string? sortBy,
        int page,
        int pageSize)
    {
        var query = _dbSet
            .Include(p => p.Category)
            .Include(p => p.Prices.Where(pr => pr.IsAvailable))
                .ThenInclude(pr => pr.Retailer)
            .AsQueryable();

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.ToLower();
            query = query.Where(p =>
                p.Name.ToLower().Contains(term) ||
                p.Description.ToLower().Contains(term) ||
                p.Brand.ToLower().Contains(term));
        }

        // Apply category filter
        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        // Apply price filters
        if (minPrice.HasValue)
        {
            query = query.Where(p => p.Prices.Any(pr => pr.IsAvailable && pr.CurrentPrice >= minPrice.Value));
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(p => p.Prices.Any(pr => pr.IsAvailable && pr.CurrentPrice <= maxPrice.Value));
        }

        // Apply retailer filter
        if (retailerIds != null && retailerIds.Any())
        {
            query = query.Where(p => p.Prices.Any(pr => pr.IsAvailable && retailerIds.Contains(pr.RetailerId)));
        }

        // Get total count before paging
        var totalCount = await query.CountAsync();

        // Apply sorting
        query = sortBy?.ToLower() switch
        {
            "price_asc" => query.OrderBy(p => p.Prices.Where(pr => pr.IsAvailable).Min(pr => pr.CurrentPrice)),
            "price_desc" => query.OrderByDescending(p => p.Prices.Where(pr => pr.IsAvailable).Min(pr => pr.CurrentPrice)),
            "newest" => query.OrderByDescending(p => p.CreatedAt),
            "discount" => query.OrderByDescending(p => p.Prices.Where(pr => pr.IsAvailable).Max(pr => pr.DiscountPercent ?? 0)),
            _ => query.OrderByDescending(p => p.CreatedAt)
        };

        // Apply paging
        var products = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (products, totalCount);
    }
}
