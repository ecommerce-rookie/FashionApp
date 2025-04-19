using static StoreFront.Domain.Enums.ProductEnums;

namespace StoreFront.Domain.Models.ProductModels.Responses
{
    public class ProductPreviewResponseModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal UnitPrice { get; set; }

        public decimal PurchasePrice { get; set; }

        public ProductStatus Status { get; set; }

        public string? Image { get; set; }

        public bool IsNew { get; set; }

        public string Slug { get; set; } = string.Empty;
    }
}
