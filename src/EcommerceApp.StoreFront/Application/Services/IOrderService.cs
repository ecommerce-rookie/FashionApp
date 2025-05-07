using Refit;
using StoreFront.Domain.Models.Common;
using StoreFront.Domain.Models.OrderModels.Requests;

namespace StoreFront.Application.Services
{
    public interface IOrderService
    {
        [Post("/orders")]
        Task<ApiResponse<APIResponse>> CreateOrder([Body] CheckoutRequest checkoutRequest);
    }
}
