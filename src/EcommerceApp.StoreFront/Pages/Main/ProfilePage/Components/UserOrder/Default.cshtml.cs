using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StoreFront.Application.Services;
using StoreFront.Domain.Models.OrderModels.Requests;
using StoreFront.Domain.Models.OrderModels.Responses;

namespace StoreFront.Pages.Main.ProfilePage.Components.UserOrder
{
    public class UserOrderViewComponent : ViewComponent
    {
        private readonly IOrderService _orderService;
        private readonly SafeApiCaller _safeApiCaller;

        public UserOrderViewComponent(IOrderService orderService, SafeApiCaller safeApiCaller)
        {
            _orderService = orderService;
            _safeApiCaller = safeApiCaller;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _safeApiCaller.CallListSafeAsync<OrderResponse>(
                    () => _orderService.GetOrders(new OrderRequest()
                    {
                        Page = 1,
                        EachPage = 99999
                    })
                );

            return View(result);
        }
    }
}
