using System.Text.Json.Serialization;
using static StoreFront.Domain.Enums.ProductEnums;
using static StoreFront.Domain.Enums.UserEnums;

namespace StoreFront.Domain.Models.ProductModels.Request
{
    public class ProductUpdateRequest
    {
        [JsonIgnore]
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public decimal? UnitPrice { get; set; }

        public decimal? PurchasePrice { get; set; }

        public string? Description { get; set; }

        public ProductStatus? Status { get; set; }

        public int? CategoryId { get; set; }

        public int? Quantity { get; set; }

        public List<string>? Sizes { get; set; }

        public Gender? Gender { get; set; }

        public IEnumerable<ImageFileRequestModel> Files { get; set; } = new List<ImageFileRequestModel>(); // Use for upload image

        public IEnumerable<string> Images { get; set; } = new List<string>();
    }
}
