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

        [Get("/products/{id}")]
        Task<APIResponse<ProductResponseModel>> GetProduct(Guid id);

        [Post("/products")]
        Task<APIResponse> CreateProduct([Body] ProductCreateRequest request);

        [Put("/products/{id}")]
        Task<APIResponse> UpdateProduct(Guid id, [Body] ProductUpdateRequest request);

        [Delete("/products/{id}")]
        Task<APIResponse> DeleteProduct(Guid id);
    }
}
