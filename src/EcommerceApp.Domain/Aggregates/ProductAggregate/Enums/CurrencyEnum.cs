using System.Text.Json.Serialization;

namespace Domain.Aggregates.ProductAggregate.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CurrencyEnum
    {
        VND = 1,
        USD = 2,
        EUR = 3
    }
}
