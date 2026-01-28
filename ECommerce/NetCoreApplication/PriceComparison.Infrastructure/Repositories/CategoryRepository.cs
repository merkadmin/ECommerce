using Microsoft.EntityFrameworkCore;
using PriceComparison.Core.Entities;
using PriceComparison.Core.Interfaces;
using PriceComparison.Infrastructure.Data;

namespace PriceComparison.Infrastructure.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Category>> GetRootCategoriesAsync()
    {
        return await _dbSet
            .Include(c => c.SubCategories)
            .Where(c => c.ParentCategoryId == null)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Category>> GetSubCategoriesAsync(Guid parentId)
    {
        return await _dbSet
            .Where(c => c.ParentCategoryId == parentId)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Category?> GetWithSubCategoriesAsync(Guid id)
    {
        return await _dbSet
            .Include(c => c.SubCategories)
            .Include(c => c.ParentCategory)
            .FirstOrDefaultAsync(c => c.Id == id);
    }
}
