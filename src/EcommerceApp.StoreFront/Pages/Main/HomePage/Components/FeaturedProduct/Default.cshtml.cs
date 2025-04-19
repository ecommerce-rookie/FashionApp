using Microsoft.AspNetCore.Mvc;
using StoreFront.Application.Services;
using StoreFront.Domain.Constants;
using StoreFront.Domain.Models.PageModels;
using StoreFront.Domain.Models.ProductModels.Request;
using StoreFront.Domain.Models.ProductModels.Responses;

namespace StoreFront.Pages.Main.HomePage.Components.FeaturedProduct
{
    public class FeaturedProductViewComponent : ViewComponent
    {
        private readonly IProductService _productService;

        public FeaturedProductViewComponent(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var productTask = _productService.GetProducts(new ProductFilterParams()
            {
                Page = 1,
                EachPage = DefaultConstant.FeaturedProductCount,
                IsNew = true,
                IsSale = true,
            });

            var sellTask = _productService.GetProducts(new ProductFilterParams()
            {
                Page = 1,
                EachPage = DefaultConstant.FeaturedProductCount,
                IsSale = true,
            });

            var newTask = _productService.GetProducts(new ProductFilterParams()
            {
                Page = 1,
                EachPage = DefaultConstant.FeaturedProductCount,
                IsNew = true,
            });

            await Task.WhenAll(productTask, sellTask, newTask);

            var products = await productTask;
            var sellProducts = await sellTask;
            var newProducts = await newTask;

            var result = new FeaturedProductPageModel()
            {
                Products = products.Content ?? Enumerable.Empty<ProductPreviewResponseModel>(),
                SellProducts = sellProducts.Content ?? Enumerable.Empty<ProductPreviewResponseModel>(),
                NewProducts = newProducts.Content ?? Enumerable.Empty<ProductPreviewResponseModel>(),
            };

            return View(result);
        }

    }
}
