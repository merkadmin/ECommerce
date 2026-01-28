namespace PriceComparison.Application.DTOs;

public class WishlistItemDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductImage { get; set; } = string.Empty;
    public decimal? LowestPrice { get; set; }
    public string? Notes { get; set; }
    public DateTime AddedAt { get; set; }
}

public class AddToWishlistDto
{
    public Guid ProductId { get; set; }
    public string? Notes { get; set; }
}

public class UpdateWishlistItemDto
{
    public string? Notes { get; set; }
}
