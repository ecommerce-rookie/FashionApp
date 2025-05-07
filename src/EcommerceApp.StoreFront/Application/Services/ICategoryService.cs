using Refit;
using StoreFront.Domain.Models.CategoryModels.Requests;
using StoreFront.Domain.Models.CategoryModels.Responses;
using StoreFront.Domain.Models.Common;

namespace StoreFront.Application.Services
{
    public interface ICategoryService
    {
        [Get("/categories")]
        Task<IEnumerable<CategoryResponse>> GetCategories([Query] CategoryRequestQuery request);
       
        [Post("/categories")]
        Task<APIResponse> CreateCategory([Body] CategoryRequest request);

        [Put("/categories/{id}")]
        Task<APIResponse> UpdateCategory(int id, [Body] CategoryRequest request);

        [Delete("/categories/{id}")]
        Task<APIResponse> DeleteCategory(int id);
    }
}
