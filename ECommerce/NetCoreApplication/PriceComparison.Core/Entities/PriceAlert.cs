namespace PriceComparison.Core.Entities;

public class PriceAlert : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public decimal TargetPrice { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsTriggered { get; set; }
    public DateTime? TriggeredAt { get; set; }
}
