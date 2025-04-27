using Domain.Aggregates.UserAggregate.Enums;
using Domain.Constants;
using Domain.readonlyants;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Authentication.Configs
{
    public static class PolicyConfig
    {
        public static void AddPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(PolicyType.Moderator, policy => policy.RequireClaim(UserClaimType.Role, UserRole.Admin.ToString(), UserRole.Staff.ToString()));
                options.AddPolicy(PolicyType.Admin, policy => policy.RequireClaim(UserClaimType.Role, UserRole.Admin.ToString()));
                options.AddPolicy(PolicyType.Customer, policy => policy.RequireClaim(UserClaimType.Role, UserRole.Customer.ToString()));
                options.AddPolicy(PolicyType.Staff, policy => policy.RequireClaim(UserClaimType.Role, UserRole.Staff.ToString()).ToString());
            });
        }
    }
}
