using Infrastructure.Authentication.Settings;

namespace Infrastructure.Authentication.Services
{
    public interface IAuthenticationService
    {
        UserAuthenModel User { get; }
    }
}