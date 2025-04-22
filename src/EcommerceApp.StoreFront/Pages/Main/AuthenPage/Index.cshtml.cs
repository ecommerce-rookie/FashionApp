using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StoreFront.Application.Extensions;

namespace StoreFront.Pages.Main.AuthenPage
{
    public class IndexModel : PageModel
    {

        public void OnGet()
        {
            
        }

        public IActionResult OnGetLogin()
        {
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = "/?type=success&action=login"
            }, OpenIdConnectDefaults.AuthenticationScheme);
        }

        public IActionResult OnGetLogout()
        {
            // Clear session data
            HttpContext.Session.SignOut();

            return SignOut(new AuthenticationProperties
            {
                RedirectUri = "/?type=success&action=logout"
            },
            CookieAuthenticationDefaults.AuthenticationScheme,
            OpenIdConnectDefaults.AuthenticationScheme);
        }

    }
}
