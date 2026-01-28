namespace PriceComparison.Application.DTOs;

public class PriceHistoryDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid RetailerId { get; set; }
    public string RetailerName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime RecordedAt { get; set; }
}

public class PriceHistoryChartDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public List<PriceHistorySeriesDto> Series { get; set; } = new();
    public decimal LowestEver { get; set; }
    public decimal HighestEver { get; set; }
    public decimal AveragePrice { get; set; }
}

public class PriceHistorySeriesDto
{
    public Guid RetailerId { get; set; }
    public string RetailerName { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public List<PricePointDto> DataPoints { get; set; } = new();
}

public class PricePointDto
{
    public DateTime Date { get; set; }
    public decimal Price { get; set; }
}
