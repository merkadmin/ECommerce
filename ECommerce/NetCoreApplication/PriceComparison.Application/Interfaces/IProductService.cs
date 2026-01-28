using PriceComparison.Application.DTOs;

namespace PriceComparison.Application.Interfaces;

public interface IProductService
{
    Task<ProductDetailDto?> GetByIdAsync(Guid id);
    Task<PagedResult<ProductDto>> GetProductsAsync(
        string? search,
        Guid? categoryId,
        decimal? minPrice,
        decimal? maxPrice,
        List<Guid>? retailerIds,
        string? sortBy,
        int page,
        int pageSize);
    Task<List<ProductDto>> GetTrendingAsync(int count = 10);
    Task<List<ProductDto>> GetDealsAsync(int count = 10);
    Task<ProductDto> CreateAsync(CreateProductDto dto);
    Task<ProductDto?> UpdateAsync(Guid id, UpdateProductDto dto);
    Task<bool> DeleteAsync(Guid id);
}
