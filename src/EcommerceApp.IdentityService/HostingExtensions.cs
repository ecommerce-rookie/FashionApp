using Duende.IdentityServer;
using Duende.IdentityServer.Services;
using EcommerceApp.IdentityService.Data;
using EcommerceApp.IdentityService.Models;
using IdentityService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace EcommerceApp.IdentityService
{
    internal static class HostingExtensions
    {
        public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddRazorPages();

            //builder.Services.AddScoped<IProfileService, CustomProfileService>();

            // Config database
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));

            // Config asp.net core Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Config IdentityServer
            builder.Services
                .AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;

                    options.EmitStaticAudienceClaim = true;
                })
                .AddInMemoryClients(Config.Clients)
                .AddInMemoryApiScopes(Config.ApiScopes)
                .AddInMemoryApiResources(Config.ApiResources)
                .AddInMemoryIdentityResources(Config.IdentityResources)
                .AddDeveloperSigningCredential()
                .AddAspNetIdentity<ApplicationUser>();

            //builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, CustomProfileService>();
            //builder.Services.AddScoped<IProfileService, CustomProfileService>();


            builder.Services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    options.ClientId = "195772379007-5orn6uk7tvg8ndh8dguqreel0l5957c1.apps.googleusercontent.com";
                    options.ClientSecret = "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";
                    options.Scope.Add("email");
                    options.Scope.Add("profile");

                    options.ClaimActions.MapJsonKey("urn:google:picture", "picture", "url");
                });

            return builder.Build();
        }

        public static WebApplication ConfigurePipeline(this WebApplication app)
        {
            app.UseSerilogRequestLogging();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthorization();

            app.MapRazorPages()
                .RequireAuthorization();

            return app;
        }
    }
}