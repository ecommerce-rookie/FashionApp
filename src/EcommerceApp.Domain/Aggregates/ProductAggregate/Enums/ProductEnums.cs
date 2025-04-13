using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Domain.Aggregates.ProductAggregate.Enums
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
}
