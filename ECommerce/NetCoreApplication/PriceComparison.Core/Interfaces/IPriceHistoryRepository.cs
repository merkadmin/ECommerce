using PriceComparison.Core.Entities;

namespace PriceComparison.Core.Interfaces;

public interface IPriceHistoryRepository : IRepository<PriceHistory>
{
    Task<IEnumerable<PriceHistory>> GetByProductIdAsync(Guid productId, int days = 30);
    Task<IEnumerable<PriceHistory>> GetByProductAndRetailerAsync(Guid productId, Guid retailerId, int days = 30);
}
