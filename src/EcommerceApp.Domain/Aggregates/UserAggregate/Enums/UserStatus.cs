using System.Text.Json.Serialization;

namespace Domain.Aggregates.UserAggregate.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum UserStatus
    {
        NotVerify = 1,
        Active = 2,
        Banned = 3,
        Deleted = 4
    }
}
