using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Repositories.BaseRepositories;

namespace Domain.Repositories
{
    public interface IImageProductRepository : ISqlRepository<ImageProduct>
    {
    }
}
