using Domain.Constants.Common;
using Duende.IdentityModel;
using Duende.IdentityServer.Models;

namespace EcommerceApp.IdentityService
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email()
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            [
                new(AuthScope.Read, "Read Access to API"),
                new(AuthScope.Write, "Write Access to API"),
                new(AuthScope.All, "Read and Write Access to API")
            ];

        public static IEnumerable<ApiResource> ApiResources =>
        [
            new()
            {
                Name = "api",
                DisplayName = "Ecommerce API",
                Scopes = { AuthScope.Read, AuthScope.Write, "profile", "openid" },
                UserClaims = new[] { JwtClaimTypes.Role, JwtClaimTypes.Confirmation, JwtClaimTypes.PreferredUserName, 
                    JwtClaimTypes.EmailVerified, JwtClaimTypes.Email },
            }
        ];

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                // m2m client credentials flow client
                new Client
                {
                    ClientId = "m2m.client",
                    ClientName = "Client Credentials Client",

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

                    AllowedScopes = { "scope1" }
                },

                // interactive client using code flow + pkce
                new Client
                {
                    ClientId = "ecommerce-api",
                    ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },

                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    RequirePkce = true,
                    RequireClientSecret = false,

                    RedirectUris = { "https://localhost:7031/auth/callback" },
                    FrontChannelLogoutUri = "https://localhost:7031/signout-oidc",
                    PostLogoutRedirectUris = { "https://localhost:7031/auth/logout" },

                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "scope2" },
                    AccessTokenLifetime = 60 * 30, // second unit
                },

                new Client
                {
                    ClientId = "postman-client",
                    ClientName = "Postman Client",
                    ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = true,

                    RedirectUris = { "https://oauth.pstmn.io/v1/callback" }, // Postman callback


                    AllowedScopes = { "openid", "profile", "api", "offline_access" },
                    RequireConsent = false,
                    AllowedCorsOrigins = { "https://oauth.pstmn.io" },

                    AllowAccessTokensViaBrowser = true,
                    AllowOfflineAccess = true,
                    AccessTokenLifetime = 3600,
                },

                new Client
                {
                    ClientId = "swagger-client",
                    ClientSecrets = { new Secret("swagger-secret".Sha256()) },
                    ClientName = "Swagger UI Client",

                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,

                    RedirectUris = { "https://localhost:7031/ap1/v1", "https://localhost:7031/swagger/oauth2-redirect.html", "https://localhost:7031/scalar/v1" },

                    AllowedScopes = { "openid", "profile", AuthScope.Read, AuthScope.Write, "offline_access", "cnf" },
                    RequireConsent = true,
                    AllowedCorsOrigins = { "https://localhost:7031" },

                    AllowOfflineAccess = true,
                    AllowAccessTokensViaBrowser = true,
                    AccessTokenLifetime = 3600
                },

                new Client 
                {
                    ClientId = "store-front",
                    ClientSecrets = { new Secret("store-front-secret".Sha256()) },
                    ClientName = "Store Front Client",

                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,

                    RedirectUris = { "https://localhost:7101/auth/callback" },

                    AllowedScopes = { "openid", "profile", "offline_access" },
                    RequireConsent = false,
                    AllowedCorsOrigins = { "https://localhost:7101" },

                    AllowOfflineAccess = true,
                }

            };
    }
}
