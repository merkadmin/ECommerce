using PriceComparison.Core.Entities;

namespace PriceComparison.Core.Interfaces;

public interface ICategoryRepository : IRepository<Category>
{
    Task<IEnumerable<Category>> GetRootCategoriesAsync();
    Task<IEnumerable<Category>> GetSubCategoriesAsync(Guid parentId);
    Task<Category?> GetWithSubCategoriesAsync(Guid id);
}
