using Application.Features.UserFeatures.Models;

namespace Application.Features.ProductFeatures.Models
{
    public class ProductPreviewManageResponesModel : ProductPreviewResponseModel
    {
        public string? CategoryName { get; set; }
        public int ReviewCount { get; set; }
        public decimal Star { get; set; }
    }
}
