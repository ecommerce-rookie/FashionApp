using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Models.Common;
using Domain.Repositories.BaseRepositories;

namespace Domain.Repositories
{
    public interface IProductRepository : ISqlRepository<Product>
    {
        /// <summary>
        /// Get all products for management by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Product?> GetManageById(Guid id);

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

        /// <summary>
        /// Get all products for management
        /// </summary>
        /// <param name="page"></param>
        /// <param name="eachPage"></param>
        /// <param name="categories"></param>
        /// <param name="sizes"></param>
        /// <returns></returns>
        Task<PagedList<Product>> GetManageProducts(int page, int eachPage, bool? isDeleted, string? search, 
            IEnumerable<int>? categories, IEnumerable<string>? sizes);

        /// <summary>
        /// Get product detail for management
        /// </summary>
        /// <param name="slug"></param>
        /// <returns></returns>
        Task<Product?> GetManageDetail(string slug);

        /// <summary>
        /// Add a range of images to the database.
        /// </summary>
        /// <param name="images"></param>
        /// <returns></returns>
        Task AddRangeImage(IEnumerable<ImageProduct> images);
    }
}
