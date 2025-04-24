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
        Task<Product?> GetDetail(string slug);

        /// <summary>
        /// Get recommend product by slug
        /// </summary>
        /// <param name="slug"></param>
        /// <returns></returns>
        Task<IEnumerable<Product>> GetRecommendProduct(string slug, int eachPage);

        /// <summary>
        /// Get best seller product
        /// </summary>
        /// <param name="eachPage"></param>
        /// <returns></returns>
        Task<IEnumerable<Product>> GetBestSeller(int eachPage);

        /// <summary>
        /// Delete a range of images from the database.
        /// </summary>
        /// <param name="images">Image Product</param>
        /// <returns></returns>
        Task DeleteImages(IEnumerable<ImageProduct> images);
    }
}
