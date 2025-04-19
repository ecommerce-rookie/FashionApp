using Microsoft.AspNetCore.Mvc;
using StoreFront.Application.Services;
using StoreFront.Domain.Constants;

namespace StoreFront.Pages.Main.ProductPage.Components.RecommendProduct
{
    public class RecommendProductViewComponent : ViewComponent
    {
        private readonly IProductService _productService;

        public RecommendProductViewComponent(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string slug)
        {
            var products = await _productService.GetRecommendProducts(slug, DefaultConstant.RecommendProductCount);

            return View(products);
        }
    }
}
