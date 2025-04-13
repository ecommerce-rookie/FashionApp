using ASM.Application.Infrastructure.Persistence.Interceptors;
using Domain.Models.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Contexts;
using Persistence.SeedWorks.Abstractions;

namespace Persistence
{
    public static class DependencyInjection
    {
        public static void AddPersistance(this WebApplicationBuilder builder)
        {
            var postgresSetting = builder.Configuration.GetSection(nameof(PostgresSetting)).Get<PostgresSetting>() ?? throw new Exception($"{nameof(PostgresSetting)} is not config!");

            //builder.Services.AddDbContextPool<EcommerceWriteContext>(options =>
            //{
            //    options
            //        .UseNpgsql(postgresSetting.ConnectionString,
            //            opt =>
            //            {
            //                opt.CommandTimeout(postgresSetting.CommandTimeout);
            //                opt.EnableRetryOnFailure(postgresSetting.RetryCount, TimeSpan.FromSeconds(postgresSetting.RetryDelay), null);
            //            });
            //    options.EnableSensitiveDataLogging();
            //    options.EnableDetailedErrors();
            //    options.AddInterceptors(new AuditableEntityInterceptor());
            //});

            //builder.Services.AddDbContext<EcommerceReadContext>(options =>
            //{
            //    options
            //        .UseNpgsql(postgresSetting.ConnectionString,
            //            opt =>
            //            {
            //                opt.CommandTimeout(postgresSetting.CommandTimeout);
            //                opt.EnableRetryOnFailure(postgresSetting.RetryCount, TimeSpan.FromSeconds(postgresSetting.RetryDelay), null);
            //                opt.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
            //            })
            //        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            //    options.EnableSensitiveDataLogging();
            //    options.EnableDetailedErrors();
            //});

            builder.Services.AddDbContext<EcommerceContext>(options =>
            {
                options
                    .UseNpgsql(postgresSetting.ConnectionString,
                        opt =>
                        {
                            opt.CommandTimeout(postgresSetting.CommandTimeout);
                            opt.EnableRetryOnFailure(postgresSetting.RetryCount, TimeSpan.FromSeconds(postgresSetting.RetryDelay), null);
                            opt.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
                        });
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            });

            builder.Services.AddScoped<IDomainEventContext, EcommerceContext>();
        }
    }
}
