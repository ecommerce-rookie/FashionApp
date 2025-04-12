namespace Domain.Repositories.BaseRepositories
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository CategoryRepository { get; }
        IImageProductRepository ImageProductRepository { get; }
        IProductRepository ProductRepository { get; }
        IOrderRepository OrderRepository { get; }
        IOrderDetailRepository OrderDetailRepository { get; }
        IUserRepository UserRepository { get; }

        Task<bool> SaveChangesAsync(CancellationToken cancellationToken);
        Task BeginTransaction(CancellationToken cancellationToken);
        Task<bool> CommitTransaction(CancellationToken cancellationToken);
    }
}
