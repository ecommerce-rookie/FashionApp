// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

using Duende.IdentityModel;
using Duende.IdentityServer;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using EcommerceApp.IdentityService.Models;
using IdentityService.Constants;
using IdentityService.Enums;
using IdentityService.Pages.Account.Login;
using IdentityService.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EcommerceApp.IdentityService.Pages.Login
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class Index : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IEventService _events;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IIdentityProviderStore _identityProviderStore;

        public ViewModel View { get; set; } = default!;

        [BindProperty]
        public InputModel Input { get; set; } = default!;

        [BindProperty]
        public RegisterUserModel RegisterUserModel { get; set; } = default!;

        public Index(
            IIdentityServerInteractionService interaction,
            IAuthenticationSchemeProvider schemeProvider,
            IIdentityProviderStore identityProviderStore,
            IEventService events,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _interaction = interaction;
            _schemeProvider = schemeProvider;
            _identityProviderStore = identityProviderStore;
            _events = events;
        }

        public async Task<IActionResult> OnGet(string? returnUrl)
        {
            await BuildModelAsync(returnUrl);

            if (View.IsExternalLoginOnly)
            {
                // we only have one option for logging in and it's an external provider
                return RedirectToPage("/ExternalLogin/Challenge", new { scheme = View.ExternalLoginScheme, returnUrl });
            }

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            // check if we are in the context of an authorization request
            var context = await _interaction.GetAuthorizationContextAsync(Input.ReturnUrl);

            // the user clicked the "cancel" button
            if (Input.Button != "login")
            {
                if (context != null)
                {
                    // This "can't happen", because if the ReturnUrl was null, then the context would be null
                    ArgumentNullException.ThrowIfNull(Input.ReturnUrl, nameof(Input.ReturnUrl));

                    // if the user cancels, send a result back into IdentityServer as if they 
                    // denied the consent (even if this client does not require consent).
                    // this will send back an access denied OIDC error response to the client.
                    await _interaction.DenyAuthorizationAsync(context, AuthorizationError.AccessDenied);

                    // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                    if (context.IsNativeClient())
                    {
                        // The client is native, so this change in how to
                        // return the response is for better UX for the end user.
                        return this.LoadingPage(Input.ReturnUrl);
                    }

                    return Redirect(Input.ReturnUrl ?? "~/");
                } else
                {
                    // since we don't have a valid context, then we just go back to the home page
                    return Redirect("~/");
                }
            }

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(Input.Username!, Input.Password!, Input.RememberLogin, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(Input.Username!);

                    if(user!.Status!.Equals(UserStatus.Banned.ToString()))
                    {
                        await _events.RaiseAsync(new UserLoginFailureEvent(user.UserName, MessageAccount.AccountBanned, clientId: context?.Client.ClientId));
                        Telemetry.Metrics.UserLoginFailure(context?.Client.ClientId, IdentityServerConstants.LocalIdentityProvider, MessageAccount.AccountBanned);
                        ModelState.AddModelError(string.Empty, MessageAccount.AccountBanned);

                        await BuildModelAsync(Input.ReturnUrl);

                        return Page();
                    } else if(user!.Status!.Equals(UserStatus.Deleted.ToString()))
                    {
                        await _events.RaiseAsync(new UserLoginFailureEvent(user.UserName, MessageAccount.AccountDeleted, clientId: context?.Client.ClientId));
                        Telemetry.Metrics.UserLoginFailure(context?.Client.ClientId, IdentityServerConstants.LocalIdentityProvider, MessageAccount.AccountDeleted);
                        ModelState.AddModelError(string.Empty, MessageAccount.AccountDeleted);

                        await BuildModelAsync(Input.ReturnUrl);

                        return Page();
                    }

                    await _events.RaiseAsync(new UserLoginSuccessEvent(user!.UserName, user.Id, user.UserName, clientId: context?.Client.ClientId));
                    Telemetry.Metrics.UserLogin(context?.Client.ClientId, IdentityServerConstants.LocalIdentityProvider);

                    if (context != null)
                    {
                        // This "can't happen", because if the ReturnUrl was null, then the context would be null
                        ArgumentNullException.ThrowIfNull(Input.ReturnUrl, nameof(Input.ReturnUrl));

                        if (context.IsNativeClient())
                        {
                            // The client is native, so this change in how to
                            // return the response is for better UX for the end user.
                            return this.LoadingPage(Input.ReturnUrl);
                        }

                        // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                        return Redirect(Input.ReturnUrl ?? "~/");
                    }

                    // request for a local page
                    if (Url.IsLocalUrl(Input.ReturnUrl))
                    {
                        return Redirect(Input.ReturnUrl);
                    } else if (string.IsNullOrEmpty(Input.ReturnUrl))
                    {
                        return Redirect("~/");
                    } else
                    {
                        // user might have clicked on a malicious link - should be logged
                        throw new ArgumentException("invalid return URL");
                    }
                }

                const string error = "invalid credentials";
                await _events.RaiseAsync(new UserLoginFailureEvent(Input.Username, error, clientId: context?.Client.ClientId));
                Telemetry.Metrics.UserLoginFailure(context?.Client.ClientId, IdentityServerConstants.LocalIdentityProvider, error);
                ModelState.AddModelError(string.Empty, LoginOptions.InvalidCredentialsErrorMessage);
            }

            // something went wrong, show form with error
            await BuildModelAsync(Input.ReturnUrl);
            return Page();
        }

        public async Task<IActionResult> OnPostRegisterUser()
        {
            ModelState.Remove("Password");
            ModelState.Remove("Username");
            if (!RegisterUserModel!.Button!.Equals("register"))
            {
                // This "can't happen", because if the Button was null, then the RegisterUserModel would be null
                ArgumentNullException.ThrowIfNull(RegisterUserModel.Button, nameof(RegisterUserModel.Button));

                return Redirect(RegisterUserModel.ReturnUrl ?? "~/");
            }


            var user = await _userManager.FindByEmailAsync(RegisterUserModel.Email!);

            if(user != null)
            {
                ModelState.AddModelError("Email", "User with this email already exists");

                return Page();
            }

            // Validate user data
            foreach(var error in ValidateUser.ValidateRegister(RegisterUserModel))
            {
                ModelState.AddModelError(error.Key, error.Value);

                return Page();
            }

            // Cretae model user
            user = new ApplicationUser()
            {
                Email = RegisterUserModel.Email,
                NormalizedEmail = RegisterUserModel.Email?.ToUpperInvariant(),
                EmailConfirmed = true,
                UserName = RegisterUserModel.Email,
                Status = UserStatus.Active.ToString()
            };

            var result= await _userManager.CreateAsync(user, RegisterUserModel.Password!);

            if(!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return Page();
            }

            await _userManager.AddClaimsAsync(user,
                [
                    new(JwtClaimTypes.Email, user.Email!)
                ]);

            await _userManager.AddToRoleAsync(user, "Customer");

            return Redirect(RegisterUserModel.ReturnUrl ?? "~/");
        }

        private async Task BuildModelAsync(string? returnUrl)
        {
            Input = new InputModel
            {
                ReturnUrl = returnUrl
            };

            RegisterUserModel = new RegisterUserModel
            {
                ReturnUrl = returnUrl
            };

            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            if (context?.IdP != null && await _schemeProvider.GetSchemeAsync(context.IdP) != null)
            {
                var local = context.IdP == Duende.IdentityServer.IdentityServerConstants.LocalIdentityProvider;

                // this is meant to short circuit the UI and only trigger the one external IdP
                View = new ViewModel
                {
                    EnableLocalLogin = local,
                };

                Input.Username = context.LoginHint;

                if (!local)
                {
                    View.ExternalProviders = new[] { new ViewModel.ExternalProvider(authenticationScheme: context.IdP) };
                }

                return;
            }

            var schemes = await _schemeProvider.GetAllSchemesAsync();

            var providers = schemes
                .Where(x => x.DisplayName != null)
                .Select(x => new ViewModel.ExternalProvider
                (
                    authenticationScheme: x.Name,
                    displayName: x.DisplayName ?? x.Name
                )).ToList();

            var dynamicSchemes = (await _identityProviderStore.GetAllSchemeNamesAsync())
                .Where(x => x.Enabled)
                .Select(x => new ViewModel.ExternalProvider
                (
                    authenticationScheme: x.Scheme,
                    displayName: x.DisplayName ?? x.Scheme
                ));
            providers.AddRange(dynamicSchemes);


            var allowLocal = true;
            var client = context?.Client;
            if (client != null)
            {
                allowLocal = client.EnableLocalLogin;
                if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Count != 0)
                {
                    providers = providers.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
                }
            }

            View = new ViewModel
            {
                AllowRememberLogin = LoginOptions.AllowRememberLogin,
                EnableLocalLogin = allowLocal && LoginOptions.AllowLocalLogin,
                ExternalProviders = providers.ToArray()
            };
        }
    }
}
