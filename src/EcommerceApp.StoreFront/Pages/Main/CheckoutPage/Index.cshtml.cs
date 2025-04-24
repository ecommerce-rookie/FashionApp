using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using StoreFront.Application.Extensions;
using StoreFront.Application.Helpers;
using StoreFront.Application.Services.CartService;
using StoreFront.Domain.Models.ProductModels.Request;

namespace StoreFront.Pages.Main.CheckoutPage
{
    [IgnoreAntiforgeryToken]
    public class IndexModel : PageModel
    {
        private readonly ICartService _cartService;

        public IndexModel(ICartService cartService)
        {
            _cartService = cartService;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAddToCartAsync([FromBody] AddToCartRequest request)
        {
            // Get userId
            var userId = User.GetUserIdFromToken();
            var result = false;

            // Save to cookie temporarily if not logged in
            if (userId == Guid.Empty)
            {
                // Get cart from cookie
                var products = Request.GetCookie<IDictionary<Guid, int>>("cart");

                // Check if cart is null
                if (products == null)
                {
                    products = new Dictionary<Guid, int>();
                }

                // Update if product existed in cart
                if (products.ContainsKey(request.ProductId))
                {
                    products[request.ProductId] += request.Quantity;
                } else
                {
                    // Add product to cart if not existed
                    products.Add(request.ProductId, request.Quantity);
                }

                Response.SetCookie("cart", products);

                result = true;
            } else
            {
                // Save to storage if logged in
                await _cartService.AddItemToCart(userId.ToString(), request.ProductId.ToString(), request.Quantity);

                // Update on session
                var totalItems = HttpContext.Session.GetNumberOfProduct();
                totalItems += request.Quantity;
                HttpContext.Session.UpdateNumberOfProduct(totalItems);

                result = true;
            }

            return new JsonResult(result);
        }

    }
}
