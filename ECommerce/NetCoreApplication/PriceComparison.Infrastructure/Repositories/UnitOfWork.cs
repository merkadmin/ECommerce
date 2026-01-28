using Microsoft.EntityFrameworkCore.Storage;
using PriceComparison.Core.Interfaces;
using PriceComparison.Infrastructure.Data;

namespace PriceComparison.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _transaction;

    private IProductRepository? _products;
    private ICategoryRepository? _categories;
    private IRetailerRepository? _retailers;
    private IPriceRepository? _prices;
    private IPriceHistoryRepository? _priceHistories;
    private IPriceAlertRepository? _priceAlerts;
    private IUserRepository? _users;
    private IWishlistRepository? _wishlists;
    private ICartRepository? _carts;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IProductRepository Products => _products ??= new ProductRepository(_context);
    public ICategoryRepository Categories => _categories ??= new CategoryRepository(_context);
    public IRetailerRepository Retailers => _retailers ??= new RetailerRepository(_context);
    public IPriceRepository Prices => _prices ??= new PriceRepository(_context);
    public IPriceHistoryRepository PriceHistories => _priceHistories ??= new PriceHistoryRepository(_context);
    public IPriceAlertRepository PriceAlerts => _priceAlerts ??= new PriceAlertRepository(_context);
    public IUserRepository Users => _users ??= new UserRepository(_context);
    public IWishlistRepository Wishlists => _wishlists ??= new WishlistRepository(_context);
    public ICartRepository Carts => _carts ??= new CartRepository(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
