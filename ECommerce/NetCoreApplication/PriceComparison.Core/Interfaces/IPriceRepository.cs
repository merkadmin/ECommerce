using PriceComparison.Core.Entities;

namespace PriceComparison.Core.Interfaces;

public interface IPriceRepository : IRepository<Price>
{
    Task<IEnumerable<Price>> GetByProductIdAsync(Guid productId);
    Task<Price?> GetByProductAndRetailerAsync(Guid productId, Guid retailerId);
    Task<decimal?> GetLowestPriceForProductAsync(Guid productId);
    Task<IEnumerable<Price>> GetAvailablePricesForProductAsync(Guid productId);
}
