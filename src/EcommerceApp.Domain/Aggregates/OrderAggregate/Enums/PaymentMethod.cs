using System.Text.Json.Serialization;

namespace Domain.Aggregates.OrderAggregate.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PaymentMethod
    {
        Cash = 1,
        PayOS = 2,
        ZaloPay = 3,
        Momo = 4,
        VnPay = 5,
    }
}
