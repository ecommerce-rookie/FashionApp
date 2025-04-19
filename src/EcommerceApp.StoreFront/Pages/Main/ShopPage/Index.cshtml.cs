using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using StoreFront.Application.Helpers;
using StoreFront.Application.Services;
using StoreFront.Domain.Constants;
using StoreFront.Domain.Enums;
using StoreFront.Domain.Models.CategoryModels.Responses;
using StoreFront.Domain.Models.Common;
using StoreFront.Domain.Models.ProductModels.Request;
using StoreFront.Domain.Models.ProductModels.Responses;
using static StoreFront.Domain.Enums.ProductEnums;

namespace StoreFront.Pages.Main.ShopPage
{
    public class ShopPageModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        
        public ShopPageModel(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        [BindProperty(SupportsGet = true)]
        public ProductFilterParams Filter { get; set; } = new()
        {
            Page = 1,
            EachPage = 12,
        };
        
        public IEnumerable<CategoryResponse> Categories { get; set; } = new List<CategoryResponse>();

        public PagedList<ProductPreviewResponseModel> Products { get; set; } = new();

        public IEnumerable<SelectListItem> Sizes { get; set; } = EnumExtension.ToSelectList<ProductSize>(useDescription: false);

        public async Task OnGet()
        {
            // Set default filter values
            Filter.Page = Filter.Page <= 0 ? 1 : Filter.Page;
            Filter.EachPage = Filter.EachPage <= 0 ? 5 : Filter.EachPage;

            var response = await _productService.GetProducts(Filter);
            Products = response.ToPagedList();
            
            Categories = await _categoryService.GetCategories();
        }


        public async Task<IActionResult> OnGetProductsPartialAsync()
        {
            var response = await _productService.GetProducts(Filter);

            return Partial(PageConstants.ShopProductList, response.ToPagedList());
        }
    }
}
