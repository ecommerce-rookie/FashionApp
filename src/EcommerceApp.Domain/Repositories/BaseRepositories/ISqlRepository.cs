using Domain.Models.Common;
using System.Linq.Expressions;

namespace Domain.Repositories.BaseRepositories
{
    public interface ISqlRepository<T> : IRepository<T> where T : class
    {
        Task<T?> GetById(dynamic[] id);

        Task UpdateRange(IEnumerable<T> entities);

        Task AddRange(IEnumerable<T> entities);

        Task Delete(dynamic[] id);

        Task Delete(T entity);

        Task<PagedList<T>> GetAll(int page, int eachPage,
                                            string sortBy, bool isAscending = false,
                                            params Expression<Func<T, object>>[]? includeProperties);
        Task<PagedList<T>> GetAll(Expression<Func<T, bool>> predicate,
                                                int page, int eachPage,
                                                string sortBy, bool isAscending = true,
                                                params Expression<Func<T, object>>[]? includeProperties);

        Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[]? includeProperties);

        Task<PagedList<TResult>> GetAll<TResult>(Expression<Func<T, bool>> predicate,
                                                Expression<Func<T, TResult>> selector,
                                                int page, int eachPage,
                                                string sortBy, bool isAscending = true,
                                                params Expression<Func<T, object>>[]? includeProperties) where TResult : class;
    }
}
