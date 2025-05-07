using System.Text.Json.Serialization;

namespace StoreFront.Domain.Enums
{
    public class OrderEnum
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
}
