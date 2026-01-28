using AutoMapper;
using PriceComparison.Application.DTOs;
using PriceComparison.Application.Interfaces;
using PriceComparison.Core.Entities;
using PriceComparison.Core.Interfaces;

namespace PriceComparison.Application.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ProductDetailDto?> GetByIdAsync(Guid id)
    {
        var product = await _unitOfWork.Products.GetByIdWithDetailsAsync(id);
        if (product == null) return null;

        var dto = _mapper.Map<ProductDetailDto>(product);
        dto.Prices = product.Prices
            .Where(p => p.IsAvailable)
            .OrderBy(p => p.CurrentPrice)
            .Select(p => _mapper.Map<RetailerPriceDto>(p))
            .ToList();

        return dto;
    }

    public async Task<PagedResult<ProductDto>> GetProductsAsync(
        string? search,
        Guid? categoryId,
        decimal? minPrice,
        decimal? maxPrice,
        List<Guid>? retailerIds,
        string? sortBy,
        int page,
        int pageSize)
    {
        var (products, totalCount) = await _unitOfWork.Products.GetPagedAsync(
            search, categoryId, minPrice, maxPrice, retailerIds, sortBy, page, pageSize);

        var productDtos = _mapper.Map<List<ProductDto>>(products);

        return new PagedResult<ProductDto>
        {
            Items = productDtos,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<List<ProductDto>> GetTrendingAsync(int count = 10)
    {
        var products = await _unitOfWork.Products.GetTrendingAsync(count);
        return _mapper.Map<List<ProductDto>>(products);
    }

    public async Task<List<ProductDto>> GetDealsAsync(int count = 10)
    {
        var products = await _unitOfWork.Products.GetDealsAsync(count);
        return _mapper.Map<List<ProductDto>>(products);
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto)
    {
        var product = _mapper.Map<Product>(dto);
        product.Id = Guid.NewGuid();
        product.CreatedAt = DateTime.UtcNow;
        product.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Products.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto?> UpdateAsync(Guid id, UpdateProductDto dto)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product == null) return null;

        if (dto.Name != null) product.Name = dto.Name;
        if (dto.Description != null) product.Description = dto.Description;
        if (dto.Brand != null) product.Brand = dto.Brand;
        if (dto.SKU != null) product.SKU = dto.SKU;
        if (dto.ImageUrl != null) product.ImageUrl = dto.ImageUrl;
        if (dto.CategoryId.HasValue) product.CategoryId = dto.CategoryId.Value;

        product.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Products.Update(product);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ProductDto>(product);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product == null) return false;

        _unitOfWork.Products.Remove(product);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
