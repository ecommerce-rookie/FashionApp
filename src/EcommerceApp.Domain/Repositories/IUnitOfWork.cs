namespace Domain.Repositories
{
    public interface IUnitOfWork : IDisposable
    {

        Task<bool> SaveChangesAsync();
        Task BeginTransaction();
        Task<bool> CommitTransaction();
    }
}
