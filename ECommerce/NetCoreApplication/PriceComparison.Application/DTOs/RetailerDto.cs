namespace PriceComparison.Application.DTOs;

public class RetailerDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    public decimal? AverageRating { get; set; }
    public bool IsActive { get; set; }
    public int ProductCount { get; set; }
}

public class CreateRetailerDto
{
    public string Name { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    public decimal? AverageRating { get; set; }
}

public class UpdateRetailerDto
{
    public string? Name { get; set; }
    public string? LogoUrl { get; set; }
    public string? BaseUrl { get; set; }
    public decimal? AverageRating { get; set; }
    public bool? IsActive { get; set; }
}
