using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Repositories.BaseRepositories;

namespace Domain.Repositories
{
    public interface IImageProductRepository : ISqlRepository<ImageProduct>
    {
        /// <summary>
        /// Delete a range of images from the database.
        /// </summary>
        /// <param name="images">Image Product</param>
        /// <returns></returns>
        Task DeleteRange(IEnumerable<ImageProduct> images);
    }
}
