using Duende.IdentityModel;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Validation;
using EcommerceApp.IdentityService.Models;
using IdentityService.Enums;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace IdentityService
{
    public class CustomResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public CustomResourceOwnerPasswordValidator(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var user = await _userManager.FindByNameAsync(context.UserName);
            if (user == null)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Invalid username or password!");
                return;
            }

            // Check if the user is banned or deleted
            if (user!.Status!.Equals(UserStatus.Banned.ToString()))
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Your account is banned!");

                return;
            } else if (user!.Status!.Equals(UserStatus.Deleted.ToString()))
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Your account is deleted!");

                return;
            }

            // Check if the user is locked out
            var result = await _signInManager.CheckPasswordSignInAsync(user, context.Password, lockoutOnFailure: true);
            if (!result.Succeeded)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Invalid username or password!");
                return;
            }

            // Check is moderator
            var roles = await _userManager.GetRolesAsync(user);
            if(roles.Contains("Customer"))
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "You are not allowed to login as a moderator!");
                return;
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("preferred_username", user.Email ?? string.Empty),
            };

            // Add custom claims based on user roles or other properties
            context.Result = new GrantValidationResult(
                subject: user.Id,
                authenticationMethod: "psw",
                claims: claims);
        }
    }
}
