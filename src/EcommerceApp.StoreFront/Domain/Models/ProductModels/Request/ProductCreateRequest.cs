using static StoreFront.Domain.Enums.ProductEnums;
using static StoreFront.Domain.Enums.UserEnums;

namespace StoreFront.Domain.Models.ProductModels.Request
{
    public class ProductCreateRequest
    {
        public string? Name { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal? PurchasePrice { get; set; }

        public string? Description { get; set; }

        public ProductStatus Status { get; set; }

        public int CategoryId { get; set; }

        public int? Quantity { get; set; }

        public List<string>? Sizes { get; set; }

        public Gender Gender { get; set; }

        public IEnumerable<IFormFile> Files { get; set; } = new List<IFormFile>();
    }
}
