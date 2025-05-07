using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using StoreFront.Application.Extensions;
using StoreFront.Application.Services;
using StoreFront.Application.Services.CartService;
using StoreFront.Domain.Constants;
using StoreFront.Domain.Models.ProductModels.Request;
using StoreFront.Domain.Models.ProductModels.Responses;

namespace StoreFront.Pages.Main.CartPage
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ICartService _cartService;
        private readonly IProductService _productService;

        public IndexModel(ICartService cartService, IProductService productService)
        {
            _cartService = cartService;
            _productService = productService;
        }

        public IDictionary<ProductPreviewResponseModel, int> Products { get; set; } = new Dictionary<ProductPreviewResponseModel, int>();

        public async Task OnGetAsync()
        {
            var userId = User.GetUserIdFromToken();

            var carts = await _cartService.GetCarts(userId.ToString());

            var products = await _productService.GetProductIds(new ProductIdsRequest()
            {
                ProductIds = carts.Keys.ToList()
            });

            Products = products.Content!.ToDictionary(x => x, x => carts[x.Id]);
        }

    }
}
