using Infrastructure.Authentication.Settings;
using Infrastructure.Shared.Extensions;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Authentication.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserAuthenModel _user = null!;

        public AuthenticationService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public UserAuthenModel User => _user ?? new UserAuthenModel()
        {
            Email = _httpContextAccessor.HttpContext?.User.GetEmailFromToken() ?? string.Empty,
            UserId = _httpContextAccessor.HttpContext?.User.GetUserIdFromToken() ?? Guid.Empty,
            Role = _httpContextAccessor.HttpContext?.User.GetRoleFromToken() ?? 0,
            SessionId = _httpContextAccessor.HttpContext?.User.GetSessionIdFromToken() ?? Guid.Empty,
            IsAuthenticated = _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false
        };

    }
}
