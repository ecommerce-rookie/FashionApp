using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace StoreFront.Configurations
{
    public static class AuthenConfig
    {
        public static void AddAuthenticationConfig(this IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60 * 24 * 30 * 12);
                options.SlidingExpiration = true;
            })
            .AddOpenIdConnect(options =>
            {
                options.Authority = "https://localhost:5001";

                options.ClientId = "store-front";
                options.ClientSecret = "store-front-secret";
                options.ResponseType = "code";
                options.RequireHttpsMetadata = true;

                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("offline_access");
                options.Scope.Add("api");

                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;

                options.CallbackPath = "/signin-oidc";
                options.SignedOutCallbackPath = "/signout-callback-oidc";
                options.SignedOutRedirectUri = "https://localhost:7101/";

                options.Events = new OpenIdConnectEvents
                {
                    OnRedirectToIdentityProvider = context =>
                    {
                        var request = context.Request;
                        if (!request.Path.StartsWithSegments("/auth"))
                        {
                            context.Response.Redirect("/");
                            context.HandleResponse();
                        }

                        return Task.CompletedTask;
                    }
                };

                options.Events.OnTokenResponseReceived = ctx =>
                {
                    var expiresIn = ctx.TokenEndpointResponse.ExpiresIn;
                    ctx.Properties!.ExpiresUtc = DateTime.UtcNow.AddSeconds(double.Parse(expiresIn));
                    return Task.CompletedTask;
                };

            });
        }
    }
}
