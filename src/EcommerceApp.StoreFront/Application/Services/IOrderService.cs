using Refit;
using StoreFront.Domain.Models.Common;
using StoreFront.Domain.Models.OrderModels.Requests;
using StoreFront.Domain.Models.OrderModels.Responses;

namespace StoreFront.Application.Services
{
    public interface IOrderService
    {
        [Post("/orders")]
        Task<ApiResponse<APIResponse>> CreateOrder([Body] CheckoutRequest checkoutRequest);

        [Get("/orders")]
        Task<ApiResponse<IEnumerable<OrderResponse>>> GetOrders([Query] OrderRequest orderRequest);
    }
}
