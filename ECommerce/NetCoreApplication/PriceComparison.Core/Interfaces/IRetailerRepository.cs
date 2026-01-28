using PriceComparison.Core.Entities;

namespace PriceComparison.Core.Interfaces;

public interface IRetailerRepository : IRepository<Retailer>
{
    Task<IEnumerable<Retailer>> GetActiveRetailersAsync();
}
