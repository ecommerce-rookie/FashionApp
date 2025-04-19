using Refit;
using StoreFront.Domain.Models.Common;
using StoreFront.Domain.Models.ProductModels.Request;
using StoreFront.Domain.Models.ProductModels.Responses;

namespace StoreFront.Application.Services
{
    public interface IProductService
    {
        [Get("/products")]
        Task<ApiResponse<IEnumerable<ProductPreviewResponseModel>>> GetProducts([Query] ProductFilterParams filter);

        [Get("/products/{slug}")]
        Task<APIResponse<ProductResponseModel>> GetProduct(string slug);

        [Post("/products")]
        Task<APIResponse> CreateProduct([Body] ProductCreateRequest request);

        [Put("/products/{slug}")]
        Task<APIResponse> UpdateProduct(string slug, [Body] ProductUpdateRequest request);

        [Delete("/products/{id}")]
        Task<APIResponse> DeleteProduct(Guid id);

        [Get("/products/recommend")]
        Task<IEnumerable<ProductPreviewResponseModel>> GetRecommendProducts([Query] string slug, [Query] int eachPage);

        [Get("/products/best-seller")]
        Task<IEnumerable<ProductPreviewResponseModel>> GetBestSellerProducts([Query] int eachPage);
    }
}
