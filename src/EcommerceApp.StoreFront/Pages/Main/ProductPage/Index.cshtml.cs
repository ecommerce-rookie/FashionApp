using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StoreFront.Application.Services;
using StoreFront.Domain.Constants;
using StoreFront.Domain.Models.ProductModels.Responses;
using System.Net;

namespace StoreFront.Pages.Main.ProductPage
{
    public class IndexModel : PageModel
    {
        private readonly IProductService _productService;

        public IndexModel(IProductService productService)
        {
            _productService = productService;
        }

        [FromRoute]
        public string Slug { get; set; } = string.Empty;

        public ProductResponseModel Product { get; set; } = new();

        public async Task OnGet()
        {
            if(string.IsNullOrEmpty(Slug))
            {
                RedirectToPage(PageConstants.ShopPage);
            }

            var response = await _productService.GetProduct(Slug);

            if (response.Status != HttpStatusCode.OK || response.Data == null)
            {
                RedirectToPage(PageConstants.ShopPage);
            }

            Product = response.Data!;
        }
    }
}
