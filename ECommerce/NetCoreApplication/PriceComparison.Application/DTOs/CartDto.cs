namespace PriceComparison.Application.DTOs;

public class CartItemDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductImage { get; set; } = string.Empty;
    public Guid? RetailerId { get; set; }
    public string? RetailerName { get; set; }
    public decimal Price { get; set; }
    public decimal? ShippingCost { get; set; }
    public int Quantity { get; set; }
    public decimal Subtotal => Price * Quantity;
    public decimal TotalWithShipping => Subtotal + (ShippingCost ?? 0);
}

public class CartSummaryDto
{
    public List<CartItemDto> Items { get; set; } = new();
    public int TotalItems => Items.Sum(i => i.Quantity);
    public decimal Subtotal => Items.Sum(i => i.Subtotal);
    public decimal TotalShipping => Items.Sum(i => i.ShippingCost ?? 0);
    public decimal GrandTotal => Subtotal + TotalShipping;
}

public class AddToCartDto
{
    public Guid ProductId { get; set; }
    public Guid? RetailerId { get; set; }
    public int Quantity { get; set; } = 1;
}

public class UpdateCartItemDto
{
    public int Quantity { get; set; }
}
