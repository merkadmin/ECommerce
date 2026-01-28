using PriceComparison.Core.Entities;

namespace PriceComparison.Core.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetByIdWithPricesAsync(Guid id);
    Task<Product?> GetByIdWithDetailsAsync(Guid id);
    Task<IEnumerable<Product>> GetByCategoryAsync(Guid categoryId);
    Task<IEnumerable<Product>> SearchAsync(string searchTerm);
    Task<IEnumerable<Product>> GetTrendingAsync(int count = 10);
    Task<IEnumerable<Product>> GetDealsAsync(int count = 10);
    Task<(IEnumerable<Product> Products, int TotalCount)> GetPagedAsync(
        string? search,
        Guid? categoryId,
        decimal? minPrice,
        decimal? maxPrice,
        List<Guid>? retailerIds,
        string? sortBy,
        int page,
        int pageSize);
}
