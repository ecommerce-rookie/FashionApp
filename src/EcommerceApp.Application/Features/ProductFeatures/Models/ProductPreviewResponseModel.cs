using Domain.Aggregates.ProductAggregate.Enums;

namespace Application.Features.ProductFeatures.Models
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
    }
}
