using System.Text.Json.Serialization;

namespace Domain.Aggregates.UserAggregate.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum UserRole
    {
        Admin = 1,
        Staff = 2,
        Customer = 3
    }
}
