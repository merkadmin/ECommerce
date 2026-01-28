namespace PriceComparison.Application.DTOs;

public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal? LowestPrice { get; set; }
    public decimal? HighestPrice { get; set; }
    public int RetailerCount { get; set; }
    public decimal? MaxDiscount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ProductDetailDto : ProductDto
{
    public List<RetailerPriceDto> Prices { get; set; } = new();
    public CategoryDto? Category { get; set; }
}

public class CreateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
}

public class UpdateProductDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Brand { get; set; }
    public string? SKU { get; set; }
    public string? ImageUrl { get; set; }
    public Guid? CategoryId { get; set; }
}
