using Domain.Constants.Common;
using Duende.IdentityModel;
using Duende.IdentityServer.Models;
using IdentityService.Models;

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
                new(AuthScope.All, "Read and Write Access to API"),
                new ApiScope("api", "Read Access to API", new[]
                {
                    JwtClaimTypes.Email,
                    JwtClaimTypes.Role,
                    JwtClaimTypes.PreferredUserName,
                    JwtClaimTypes.EmailVerified,
                    JwtClaimTypes.Confirmation
                })
            ];

        public static IEnumerable<ApiResource> ApiResources =>
        [
            new()
            {
                Name = "api",
                DisplayName = "Ecommerce API",
                Scopes = { "api", "profile", "openid", "offline_access" },
                UserClaims = new[] { JwtClaimTypes.Role, JwtClaimTypes.Confirmation, JwtClaimTypes.PreferredUserName, 
                    JwtClaimTypes.EmailVerified, JwtClaimTypes.Email },
            }
        ];

        public static IEnumerable<Client> GetClients(ClientUrls clientUrls) =>
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

                    RedirectUris = { $"{clientUrls.Api}/auth/callback" },
                    FrontChannelLogoutUri = $"{clientUrls.Api}/signout-oidc",
                    PostLogoutRedirectUris = { $"{clientUrls.Api}/auth/logout" },
                    AllowedCorsOrigins = { clientUrls.Api! },

                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "scope2" },
                    AccessTokenLifetime = 60 * 30, // second unit
                },

                new Client
                {
                    ClientId = "scalar-client",
                    ClientSecrets = { new Secret("scalar-secret".Sha256()) },
                    ClientName = "Scalar UI Client",

                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,

                    RedirectUris = { $"{clientUrls.Api}/scalar/v1" },

                    AllowedScopes = { "openid", "profile", "offline_access", "api" },
                    RequireConsent = false,
                    AllowedCorsOrigins = { clientUrls.Api! },

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

                    RedirectUris = { $"{clientUrls.StoreFront}/signin-oidc" },
                    PostLogoutRedirectUris = {
                        $"{clientUrls.StoreFront}/signout-callback-oidc"
                    },


                    AllowedScopes = { "openid", "profile", "offline_access", "api" },
                    RequireConsent = false,
                    AllowedCorsOrigins = { clientUrls.StoreFront! },

                    AllowOfflineAccess = true,
                }

            };
    }
}
