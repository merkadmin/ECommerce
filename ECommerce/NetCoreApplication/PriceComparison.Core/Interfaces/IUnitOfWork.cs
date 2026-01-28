namespace PriceComparison.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IProductRepository Products { get; }
    ICategoryRepository Categories { get; }
    IRetailerRepository Retailers { get; }
    IPriceRepository Prices { get; }
    IPriceHistoryRepository PriceHistories { get; }
    IPriceAlertRepository PriceAlerts { get; }
    IUserRepository Users { get; }
    IWishlistRepository Wishlists { get; }
    ICartRepository Carts { get; }

    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
