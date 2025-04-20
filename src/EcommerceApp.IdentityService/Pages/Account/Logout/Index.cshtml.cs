// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

using Duende.IdentityModel;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Services;
using EcommerceApp.IdentityService.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EcommerceApp.IdentityService.Pages.Logout
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class Index : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IEventService _events;

        [BindProperty]
        public string? LogoutId { get; set; }

        public Index(SignInManager<ApplicationUser> signInManager, IIdentityServerInteractionService interaction, IEventService events)
        {
            _signInManager = signInManager;
            _interaction = interaction;
            _events = events;
        }

        public async Task<IActionResult> OnGet(string? logoutId)
        {
            LogoutId = logoutId;

            var showLogoutPrompt = LogoutOptions.ShowLogoutPrompt;

            if (User.Identity?.IsAuthenticated != true)
            {
                // if the user is not authenticated, then just show logged out page
                showLogoutPrompt = false;
            } else
            {
                var context = await _interaction.GetLogoutContextAsync(LogoutId);
                if (context?.ShowSignoutPrompt == false)
                {
                    // it's safe to automatically sign-out
                    showLogoutPrompt = false;
                }
            }

            if (showLogoutPrompt == false)
            {
                // if the request for logout was properly authenticated from IdentityServer, then
                // we don't need to show the prompt and can just log the user out directly.
                return await PerformLogoutAndRedirect();
            }

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            // if the user is authenticated, then we need to sign them out
            if (User.Identity?.IsAuthenticated == true)
            {
                await _signInManager.SignOutAsync();
                await _events.RaiseAsync(new UserLogoutSuccessEvent(
                    User.GetSubjectId(), User.GetDisplayName()));
                Telemetry.Metrics.UserLogout(
                    User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value);
            }

            // if there's no current logout context, we need to create one
            return await PerformRedirectAfterLogout();
        }

        private async Task<IActionResult> PerformLogoutAndRedirect()
        {
            // if the user is not authenticated, then just show logged out page
            if (User.Identity?.IsAuthenticated == true)
            {
                await _signInManager.SignOutAsync();
                await _events.RaiseAsync(new UserLogoutSuccessEvent(
                    User.GetSubjectId(), User.GetDisplayName()));
                Telemetry.Metrics.UserLogout(
                    User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value);
            }

            return await PerformRedirectAfterLogout();
        }

        private async Task<IActionResult> PerformRedirectAfterLogout()
        {
            // if there's no current logout context, we need to create one
            LogoutId ??= await _interaction.CreateLogoutContextAsync();

            // Get context to check if we need to redirect
            var context = await _interaction.GetLogoutContextAsync(LogoutId);
            if (!string.IsNullOrEmpty(context?.PostLogoutRedirectUri))
            {
                // check if the post logout redirect URI is still valid
                return Redirect(context.PostLogoutRedirectUri);
            }

            // if we don't have a post logout redirect URI, then we just go back to the home page
            return RedirectToPage("/Account/Logout/LoggedOut", new { logoutId = LogoutId });
        }
    }
}
