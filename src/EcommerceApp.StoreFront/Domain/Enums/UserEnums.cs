using System.Text.Json.Serialization;

namespace StoreFront.Domain.Enums
{
    public class UserEnums
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum Gender
        {
            Male = 1,
            Female = 2,
            Unisex = 3,
            Other = 4
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum UserRole
        {
            Admin = 1,
            Staff = 2,
            Customer = 3
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum UserStatus
        {
            NotVerify = 1,
            Active = 2,
            Banned = 3,
            Deleted = 4
        }
    }
}
