using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using StoreFront.Application.Extensions;
using StoreFront.Application.Helpers;
using StoreFront.Application.Services;
using StoreFront.Application.Services.CartService;
using StoreFront.Domain.Models.Common;
using StoreFront.Domain.Models.OrderModels.Requests;
using StoreFront.Domain.Models.ProductModels.Request;
using StoreFront.Domain.Models.ProductModels.Responses;
using static StoreFront.Domain.Enums.OrderEnum;

namespace StoreFront.Pages.Main.CheckoutPage
{
    [IgnoreAntiforgeryToken]
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ICartService _cartService;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly SafeApiCaller _safeApiCaller;

        public IndexModel(ICartService cartService, IProductService productService, 
            IOrderService orderService, SafeApiCaller safeApiCaller)
        {
            _cartService = cartService;
            _productService = productService;
            _orderService = orderService;
            _safeApiCaller = safeApiCaller;
        }

        public IDictionary<ProductPreviewResponseModel, int> Products { get; set; } = new Dictionary<ProductPreviewResponseModel, int>();

        [BindProperty]
        public string Address { get; set; } = string.Empty;
        [BindProperty]
        public string FirstName { get; set; } = string.Empty;
        [BindProperty]
        public string LastName { get; set; } = string.Empty;
        [BindProperty]
        public PaymentMethod PaymentMethod { get; set; }

        public async Task OnGet(string? message)
        {
            var userId = User.GetUserIdFromToken();

            var carts = await _cartService.GetCarts(userId.ToString());

            var products = await _productService.GetProductIds(new ProductIdsRequest()
            {
                ProductIds = carts.Keys.ToList()
            });

            Products = products.Content!.ToDictionary(x => x, x => carts[x.Id]);

            if(!string.IsNullOrEmpty(message))
            {
                TempData.SetError(message, "Check out fail!");
            }

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

        public async Task<IActionResult> OnPostCheckOut()
        {
            var userId = User.GetUserIdFromToken();

            var carts = await _cartService.GetCarts(userId.ToString());

            var request = new CheckoutRequest()
            {
                Address = Address,
                NameReceiver = $"{FirstName} {LastName}",
                PaymentMethod = PaymentMethod,
                Carts = carts.Select(x => new CartRequest()
                {
                    ProductId = x.Key,
                    Quantity = x.Value
                })
            };

            var result = await _safeApiCaller.CallSafeAsync(
                () => _orderService.CreateOrder(request));

            if(result.IsSuccess)
            {
                // clear cart
                await _cartService.ClearCart(userId.ToString());

                // update session
                HttpContext.Session.UpdateNumberOfProduct(0);

                // redirect to order page
                return Redirect($"/products?message={result.Message}");
            }

            return Redirect($"/checkout?message={result.Message}");
        }

    }
}
