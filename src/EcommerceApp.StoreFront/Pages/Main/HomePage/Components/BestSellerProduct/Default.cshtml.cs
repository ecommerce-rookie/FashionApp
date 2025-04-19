using Microsoft.AspNetCore.Mvc;
using StoreFront.Application.Services;

namespace StoreFront.Pages.Main.HomePage.Components.BestSellerProduct
{
    public class BestSellerProductViewComponent : ViewComponent
    {
        private readonly IProductService _productService;

        public BestSellerProductViewComponent(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var products = await _productService.GetBestSellerProducts(Domain.Constants.DefaultConstant.BestSellerProductCount);

            return View(products);
        }
    }
}
