using System.Text.Json.Serialization;

namespace Domain.Aggregates.OrderAggregate.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum OrderStatus
    {
        Pending = 1,
        Processing = 2,
        Shipping = 3,
        Delivered = 4,
        Cancelled = 5
    }
}
