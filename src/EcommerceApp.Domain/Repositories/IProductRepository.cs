using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Repositories.BaseRepositories;

namespace Domain.Repositories
{
    public interface IProductRepository : ISqlRepository<Product>
    {
        Task<bool> CheckDuplicatedName(string name);
    }
}
