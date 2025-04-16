using Domain.Aggregates.UserAggregate.Enums;

namespace Infrastructure.Authentication.Settings
{
    public class UserAuthenModel
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public UserRole? Role { get; set; }
        public Guid SessionId { get; set; }
        public bool IsAuthenticated { get; set; }

    }
}
