using System.ComponentModel;
using System.Text.Json.Serialization;

namespace StoreFront.Domain.Enums
{
    public static class ProductEnums
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

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum ProductSize
        {
            S = 1,
            M = 2,
            L = 3,
            XL = 4,
            XXL = 5,
            Other = 6
        }
    }
}
