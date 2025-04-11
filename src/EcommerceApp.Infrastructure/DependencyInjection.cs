using Domain.Models.Settings;
using Infrastructure.Authentication.Services;
using Infrastructure.BackgroundServices.BackgroundTask;
using Infrastructure.BackgroundServices.Workers;
using Infrastructure.Cache;
using Infrastructure.Cache.Services;
using Infrastructure.Configurations;
using Infrastructure.Email;
using Infrastructure.Versions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using RookieShop.Infrastructure.Swagger;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddInfrastructure(this WebApplicationBuilder builder)
        {
            // Set up configuration
            builder.Services.Configure<JWTSetting>(builder.Configuration.GetSection(nameof(JWTSetting)));
            builder.Services.Configure<WorkerSetting>(builder.Configuration.GetSection(nameof(WorkerSetting)));

            // Set up caching Redis
            builder.AddRedis();

            // Set up configuration Security
            builder.Services.AddJWT(builder.Configuration);

            // Set up Swagger
            //builder.Services.AddSwagger();
            builder.AddOpenApi();

            // Set up email
            builder.Services.AddFluentEmail(builder.Configuration);

            // Set up system
            builder.AddSystem();

            // Set up background services
            builder.Services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            //builder.Services.AddHostedService<CommonWorkerService>();

            // Set up authen service
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
            builder.Services.AddSingleton<ICacheService, CacheService>();

            // Set up api versioning
            builder.Services.AddAPIVersioning();

            // Anothers
            builder.Services.AddAuthorization();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddCors();
        }
    }
}
