using Domain.Repositories;
using Domain.Repositories.BaseRepositories;
using Microsoft.EntityFrameworkCore.Storage;
using Persistence.Contexts;
using Persistence.Repository;

namespace Persistence.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EcommerceWriteContext _context;

        private readonly ICategoryRepository _categoryRepository = null!;
        private readonly IImageProductRepository _imageProductRepository = null!;
        private readonly IProductRepository _productRepository = null!;
        private readonly IOrderRepository _orderRepository = null!;
        private readonly IOrderDetailRepository _orderDetailRepository = null!;
        private readonly IUserRepository _userRepository = null!;

        public UnitOfWork(EcommerceWriteContext context)
        {
            _context = context;
        }

        public ICategoryRepository CategoryRepository => _categoryRepository ?? new CategoryRepository(_context);
        public IImageProductRepository ImageProductRepository => _imageProductRepository ?? new ImageProductRepository(_context);
        public IProductRepository ProductRepository => _productRepository ?? new ProductRepository(_context);
        public IOrderRepository OrderRepository => _orderRepository ?? new OrderRepository(_context);
        public IOrderDetailRepository OrderDetailRepository => _orderDetailRepository ?? new OrderDetailRepository(_context);
        public IUserRepository UserRepository => _userRepository ?? new UserRepository(_context);

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        public async Task BeginTransaction(CancellationToken cancellationToken)
        {
            await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task<bool> CommitTransaction(CancellationToken cancellationToken)
        {
            try
            {
                await _context.Database.CommitTransactionAsync(cancellationToken);

                return true;
            } catch
            {
                return false;
            }

        }

    }
}
