using StoreFront.Domain.Models.ProductModels.Responses;

namespace StoreFront.Domain.Models.PageModels
{
    public class FeaturedProductPageModel
    {
        public IEnumerable<ProductPreviewResponseModel> Products { get; set; } = new List<ProductPreviewResponseModel>();
        public IEnumerable<ProductPreviewResponseModel> NewProducts { get; set; } = new List<ProductPreviewResponseModel>();
        public IEnumerable<ProductPreviewResponseModel> SellProducts { get; set; } = new List<ProductPreviewResponseModel>();
    }
}
