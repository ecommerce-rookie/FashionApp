using Duende.IdentityModel;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using EcommerceApp.IdentityService.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace IdentityService
{
    public class CustomProfileService : IProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public CustomProfileService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subjectId = context.Subject?.FindFirst(JwtClaimTypes.Subject)?.Value;
            if (subjectId == null)
            {
                return;
            }

            var user = await _userManager.FindByIdAsync(subjectId);
            if (user == null)
            {
                return;
            }

            var claims = context.Subject?.Claims.ToList();

            claims?.Add(new Claim(JwtClaimTypes.EmailVerified, user.EmailConfirmed.ToString()));

            var roles = await _userManager.GetRolesAsync(user);
            claims?.Add(new Claim(JwtClaimTypes.Role, roles.ElementAt(0)));

            context.IssuedClaims = claims ?? new List<Claim>();
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var subjectId = context.Subject?.FindFirst(JwtClaimTypes.Subject)?.Value;
            if (subjectId == null)
            {
                context.IsActive = false;
                return;
            }

            var user = await _userManager.FindByIdAsync(subjectId);
            context.IsActive = (user != null);
        }
    }
}
