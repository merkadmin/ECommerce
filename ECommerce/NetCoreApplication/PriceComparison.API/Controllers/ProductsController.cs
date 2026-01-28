using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceComparison.Application.DTOs;
using PriceComparison.Application.Interfaces;

namespace PriceComparison.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IPriceComparisonService _priceComparisonService;

    public ProductsController(
        IProductService productService,
        IPriceComparisonService priceComparisonService)
    {
        _productService = productService;
        _priceComparisonService = priceComparisonService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ProductDto>>>> GetProducts(
        [FromQuery] string? search,
        [FromQuery] Guid? categoryId,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] List<Guid>? retailerIds,
        [FromQuery] string? sortBy,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _productService.GetProductsAsync(
            search, categoryId, minPrice, maxPrice, retailerIds, sortBy, page, pageSize);

        return Ok(ApiResponse<PagedResult<ProductDto>>.SuccessResponse(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ProductDetailDto>>> GetProduct(Guid id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null)
        {
            return NotFound(ApiResponse<ProductDetailDto>.FailResponse("Product not found"));
        }

        return Ok(ApiResponse<ProductDetailDto>.SuccessResponse(product));
    }

    [HttpGet("{id:guid}/compare")]
    public async Task<ActionResult<ApiResponse<PriceComparisonDto>>> ComparePrice(Guid id)
    {
        var comparison = await _priceComparisonService.CompareProductPricesAsync(id);
        if (comparison == null)
        {
            return NotFound(ApiResponse<PriceComparisonDto>.FailResponse("Product not found"));
        }

        return Ok(ApiResponse<PriceComparisonDto>.SuccessResponse(comparison));
    }

    [HttpGet("{id:guid}/history")]
    public async Task<ActionResult<ApiResponse<PriceHistoryChartDto>>> GetPriceHistory(
        Guid id,
        [FromQuery] int days = 30)
    {
        var history = await _priceComparisonService.GetPriceHistoryAsync(id, days);
        return Ok(ApiResponse<PriceHistoryChartDto>.SuccessResponse(history));
    }

    [HttpGet("trending")]
    public async Task<ActionResult<ApiResponse<List<ProductDto>>>> GetTrending([FromQuery] int count = 10)
    {
        var products = await _productService.GetTrendingAsync(count);
        return Ok(ApiResponse<List<ProductDto>>.SuccessResponse(products));
    }

    [HttpGet("deals")]
    public async Task<ActionResult<ApiResponse<List<ProductDto>>>> GetDeals([FromQuery] int count = 10)
    {
        var products = await _productService.GetDealsAsync(count);
        return Ok(ApiResponse<List<ProductDto>>.SuccessResponse(products));
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ProductDto>>> CreateProduct([FromBody] CreateProductDto dto)
    {
        var product = await _productService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id },
            ApiResponse<ProductDto>.SuccessResponse(product, "Product created successfully"));
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ProductDto>>> UpdateProduct(Guid id, [FromBody] UpdateProductDto dto)
    {
        var product = await _productService.UpdateAsync(id, dto);
        if (product == null)
        {
            return NotFound(ApiResponse<ProductDto>.FailResponse("Product not found"));
        }

        return Ok(ApiResponse<ProductDto>.SuccessResponse(product, "Product updated successfully"));
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse>> DeleteProduct(Guid id)
    {
        var result = await _productService.DeleteAsync(id);
        if (!result)
        {
            return NotFound(ApiResponse.FailResponse("Product not found"));
        }

        return Ok(ApiResponse.SuccessResponse("Product deleted successfully"));
    }
}
