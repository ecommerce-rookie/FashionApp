using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Repositories.BaseRepositories;

namespace Domain.Repositories
{
    public interface IProductRepository : ISqlRepository<Product>
    {
        /// <summary>
        /// Check if the product name is duplicated
        /// </summary>
        /// <param name="name">Name of product</param>
        /// <returns></returns>
        Task<bool> CheckDuplicatedName(string name);

        /// <summary>
        /// Get product detail by id
        /// </summary>
        /// <param name="id">Id of product</param>
        /// <returns></returns>
        Task<Product?> GetDetail(Guid id);
    }
}
