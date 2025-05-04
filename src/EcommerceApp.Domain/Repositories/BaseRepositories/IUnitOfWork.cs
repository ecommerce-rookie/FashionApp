namespace Domain.Repositories.BaseRepositories
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository CategoryRepository { get; }
        IProductRepository ProductRepository { get; }
        IOrderRepository OrderRepository { get; }
        IUserRepository UserRepository { get; }
        IFeedbackRepository FeedbackRepository { get; }

        Task<bool> SaveChangesAsync(CancellationToken cancellationToken);
        Task BeginTransaction(CancellationToken cancellationToken);
        Task RollbackTransaction(CancellationToken cancellationToken);
        Task<bool> CommitTransaction(CancellationToken cancellationToken);
        Task<T> ExecuteTransactionalAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken);
    }
}
