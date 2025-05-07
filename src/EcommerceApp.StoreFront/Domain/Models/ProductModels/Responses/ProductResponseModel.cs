using StoreFront.Domain.Models.UserModels.Responses;
using static StoreFront.Domain.Enums.ProductEnums;
using static StoreFront.Domain.Enums.UserEnums;

namespace StoreFront.Domain.Models.ProductModels.Responses
{
    public class ProductResponseModel
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal PurchasePrice { get; set; }

        public string? Description { get; set; }

        public ProductStatus Status { get; set; }

        public int CategoryId { get; set; }

        public string? CategoryName { get; set; }

        public int? Quantity { get; set; }

        public List<string>? Colors { get; set; }

        public List<string>? Sizes { get; set; }

        public Gender Gender { get; set; }

        public IEnumerable<string> Images { get; set; } = new List<string>();

        public AuthorResponse? Author { get; set; }

        public bool IsNew { get; set; }

        public int Star { get; set; }

        public int ReviewCount { get; set; }
    }
}
