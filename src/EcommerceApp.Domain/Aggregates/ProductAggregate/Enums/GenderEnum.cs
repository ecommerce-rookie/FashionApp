using System.Text.Json.Serialization;

namespace Domain.Aggregates.ProductAggregate.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Gender
    {
        Male = 1,
        Female = 2,
        Unisex = 3,
        Other = 4
    }
}
