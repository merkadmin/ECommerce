namespace PriceComparison.Application.DTOs;

public class PriceAlertDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductImage { get; set; } = string.Empty;
    public decimal TargetPrice { get; set; }
    public decimal? CurrentLowestPrice { get; set; }
    public bool IsActive { get; set; }
    public bool IsTriggered { get; set; }
    public DateTime? TriggeredAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateAlertDto
{
    public Guid ProductId { get; set; }
    public decimal TargetPrice { get; set; }
}

public class UpdateAlertDto
{
    public decimal? TargetPrice { get; set; }
    public bool? IsActive { get; set; }
}
