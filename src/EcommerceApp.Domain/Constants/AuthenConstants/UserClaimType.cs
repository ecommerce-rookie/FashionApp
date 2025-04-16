using System.Security.Claims;

namespace Domain.readonlyants
{
    public class UserClaimType
    {
        public const string Provider = "http://schemas.microsoft.com/identity/claims/identityprovider";
        public const string UserId = ClaimTypes.NameIdentifier;
        public const string Role = ClaimTypes.Role;
        public const string Status = "status";
        public const string SessionId = "sid";

        public const string Avatar = nameof(Avatar);
        public const string Email = "email";
        public const string UserName = "preferred_username";
    }
}
