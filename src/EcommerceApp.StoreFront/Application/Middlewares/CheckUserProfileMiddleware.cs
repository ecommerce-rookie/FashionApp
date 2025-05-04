using Application.Features.UserFeatures.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using StoreFront.Application.Extensions;
using StoreFront.Application.Services;
using StoreFront.Application.Services.CartService;
using static StoreFront.Domain.Enums.UserEnums;

namespace StoreFront.Application.Middlewares
{
    public class CheckUserProfileMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CheckUserProfileMiddleware> _logger;

        public CheckUserProfileMiddleware(RequestDelegate next, ILogger<CheckUserProfileMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IUserService userService)
        {
            
            var user = context.User;
            // Check if user is authenticated and session is not authenticated
            if ((user.Identity?.IsAuthenticated is true) && !context.Session.IsAuthenticated())
            {
                var cartService = context.RequestServices.GetRequiredService<ICartService>();
                try
                {
                    // Migrate cart from cookie to db
                    await TransferCart(context, cartService);

                    var response = await userService.GetAuthorInfo();
                    var path = context.Request.Path.Value?.ToLower() ?? "";

                    if (!response.IsSuccess && !path.StartsWith("/onboarding"))
                    {
                        _logger.LogWarning("User profile is not completed, redirecting to onboarding page");
                        // Redirect to onboarding page if user profile is not completed
                        context.Response.Redirect("/onboarding");

                        return;
                    } else if(response.IsSuccess && response.Data != null)
                    {
                        _logger.LogInformation("User profile is completed, setting session");

                        // Check status account
                        await ForceSignOut(response.Data, context);

                        var role = context.User.GetRoleFromToken();
                        // Set to session if user profile is existed
                        context.Session.SignIn(response.Data, await cartService.CountProduct(response.Data!.Id.ToString()), role!.Value != UserRole.Customer);
                    }
                } catch(Exception e)
                {
                    _logger.LogError(e, "Error while checking user profile");
                }
            }

            await _next(context);
        }

        private async Task ForceSignOut(AuthorResponse author, HttpContext context)
        {
            if (author.Status!.Equals(UserStatus.Banned.ToString()) || author.Status!.Equals(UserStatus.Deleted.ToString()))
            {
                // Signout user if account is banned
                _logger.LogWarning("User account is banned, redirecting to error page");

                context.Session.Clear();

                // Redirect đến Razor Page logout handler — nơi có SignOut chuẩn
                context.Response.Redirect($"/auth?handler=Logout&type={author.Status}&action=force-logout");
            }
        }

        private async Task TransferCart(HttpContext context, ICartService cartService)
        {
            // migrate cart from cookie to db
            var products = context.Request.GetCookie<IDictionary<Guid, int>>("cart");

            if (products == null || !products.Any())
            {
                return;
            }

            await cartService.AddItemsToCart(context.User.GetUserIdFromToken().ToString(), products);

            // Clear cart on cookie
            context.Response.Cookies.Delete("cart");
        }
    }

}
