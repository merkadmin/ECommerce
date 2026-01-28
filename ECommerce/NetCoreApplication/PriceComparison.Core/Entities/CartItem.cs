namespace PriceComparison.Core.Entities;

public class CartItem : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public Guid? RetailerId { get; set; }
    public Retailer? Retailer { get; set; }
    public int Quantity { get; set; } = 1;
}
