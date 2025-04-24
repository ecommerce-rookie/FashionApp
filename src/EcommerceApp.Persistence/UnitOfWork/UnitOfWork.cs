using Domain.Repositories;
using Domain.Repositories.BaseRepositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;
using Persistence.Repository;

namespace Persistence.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        public readonly EcommerceContext _context;

        private readonly ICategoryRepository _categoryRepository = null!;
        private readonly IProductRepository _productRepository = null!;
        private readonly IOrderRepository _orderRepository = null!;
        private readonly IUserRepository _userRepository = null!;
        private readonly IFeedbackRepository _feedbackRepository = null!;

        public UnitOfWork(EcommerceContext context)
        {
            _context = context;
        }

        public ICategoryRepository CategoryRepository => _categoryRepository ?? new CategoryRepository(_context);
        public IProductRepository ProductRepository => _productRepository ?? new ProductRepository(_context);
        public IOrderRepository OrderRepository => _orderRepository ?? new OrderRepository(_context);
        public IUserRepository UserRepository => _userRepository ?? new UserRepository(_context);
        public IFeedbackRepository FeedbackRepository => _feedbackRepository ?? new FeedbackRepository(_context);

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

        public async Task<T> ExecuteTransactionalAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken)
        {
            // Get execution strategy from EF Core
            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                // if not exist, creating new transaction
                if (_context.Database.CurrentTransaction == null)
                {
                    await _context.Database.BeginTransactionAsync(cancellationToken);
                }

                // Excute logic
                var result = await operation();

                try
                {
                    // Commit if open transaction
                    if (_context.Database.CurrentTransaction != null)
                    {
                        await _context.Database.CommitTransactionAsync(cancellationToken);
                    }
                } catch(Exception ex) 
                {
                    // If error, rollback
                    if (_context.Database.CurrentTransaction != null)
                    {
                        await _context.Database.RollbackTransactionAsync(cancellationToken);
                    }

                    Console.WriteLine(ex.ToString());
                }

                return result;
            });
        }


    }
}
