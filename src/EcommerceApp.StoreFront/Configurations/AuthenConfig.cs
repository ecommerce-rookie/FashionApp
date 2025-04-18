﻿using Microsoft.AspNetCore.Authentication.Cookies;
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
            .AddCookie()
            .AddOpenIdConnect(options =>
            {
                options.Authority = "https://localhost:5001";
                options.ClientId = "store-front";
                options.ClientSecret = "store-front-secret";
                options.ResponseType = "code";
                options.RequireHttpsMetadata = true;
                options.CallbackPath = "/auth/callback";

                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("offline_access");

                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;
            });
        }
    }
}
