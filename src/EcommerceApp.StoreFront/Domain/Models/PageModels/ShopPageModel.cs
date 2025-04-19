using StoreFront.Domain.Models.CategoryModels.Responses;
using StoreFront.Domain.Models.Common;
using StoreFront.Domain.Models.ProductModels.Request;
using StoreFront.Domain.Models.ProductModels.Responses;

namespace StoreFront.Domain.Models.PageModels;

public class ShopPageModel
{
    public ProductFilterParams Filter { get; set; } = new();
    public CategoryResponse Category { get; set; } = new();
}
