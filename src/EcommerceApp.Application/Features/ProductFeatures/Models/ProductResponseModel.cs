using Application.Features.UserFeatures.Models;
using Domain.Aggregates.ProductAggregate.Enums;

namespace Application.Features.ProductFeatures.Models
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

        public AuthorResponseModel? Author { get; set; }

        public bool IsNew { get; set; }
    }
}
