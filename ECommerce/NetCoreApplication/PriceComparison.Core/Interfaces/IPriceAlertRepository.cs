using PriceComparison.Core.Entities;

namespace PriceComparison.Core.Interfaces;

public interface IPriceAlertRepository : IRepository<PriceAlert>
{
    Task<IEnumerable<PriceAlert>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<PriceAlert>> GetActiveAlertsAsync();
    Task<PriceAlert?> GetByUserAndProductAsync(Guid userId, Guid productId);
}
