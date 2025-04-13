using Application.Behaviors;
using Domain.Repositories.BaseRepositories;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Persistence.UnitOfWork;
using RookieShop.Persistence;
using System.Reflection;

namespace Application
{
    public static class DependencyInjection
    {
        public static void AddApplication(this IServiceCollection services)
        {
            // Set up AutoMapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Set up Validation
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(TxBehavior<,>), ServiceLifetime.Scoped);
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>), ServiceLifetime.Scoped);
                cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>));
            });

            // Set up unitofwork
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}
