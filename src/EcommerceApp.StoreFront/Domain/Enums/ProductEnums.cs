using System.ComponentModel;
using System.Text.Json.Serialization;

namespace StoreFront.Domain.Enums
{
    public class ProductEnums
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum ProductStatus
        {
            [Description("Available")]
            Available = 1,
            [Description("Out of stock")]
            OutOfStock = 2,
            [Description("Blocked")]
            Blocked = 3,
            [Description("Deleted")]
            Deleted = 4,
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum ProductSortBy
        {
            Name = 1,
            CreatedAt = 2,
            Price = 3,
        }
    }
}
