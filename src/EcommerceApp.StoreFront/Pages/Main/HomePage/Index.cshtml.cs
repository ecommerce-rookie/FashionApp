using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StoreFront.Application.Helpers;
using StoreFront.Application.Services;
using StoreFront.Domain.Models.ProductModels.Request;

namespace StoreFront.Pages.Main.HomePage
{
    public class HomePageModel : PageModel
    {
        private readonly IProductService _productService;

        public HomePageModel(IProductService productService)
        {
            _productService = productService;
        }

        public async Task OnGet()
        {

            var response = await _productService.GetProducts(new ProductFilterParams()
            {
                Page = 1,
                EachPage = 10
            });

            var products = response.ToPagedList();


            if (products != null)
            {
                foreach (var product in products)
                {
                    Console.WriteLine($"Product Name: {product.Name}, Id: {product.Id}");
                }
            } else
            {
                Console.WriteLine("No products found.");
            }
        }
    }
}
