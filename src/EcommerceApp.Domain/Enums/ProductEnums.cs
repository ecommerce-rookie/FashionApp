using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Domain.Enums
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
            [Description("Unavailable")]
            Unavailable = 3,
            [Description("Blocked")]
            Blocked = 4,
            [Description("Deleted")]
            Deleted = 5,
        }
    }
}
