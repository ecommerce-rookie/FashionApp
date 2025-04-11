using Domain.Repositories;

namespace Persistence.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {

        public UnitOfWork()
        {
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return true;
        }

        public Task BeginTransaction()
        {

            return Task.CompletedTask;
        }

        public async Task<bool> CommitTransaction()
        {
            try
            {

                return true;
            } catch
            {
                return false;
            }

        }

    }
}
