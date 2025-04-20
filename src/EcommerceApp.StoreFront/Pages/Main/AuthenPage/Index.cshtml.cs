using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;

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
                RedirectUri = "/"
            }, OpenIdConnectDefaults.AuthenticationScheme);
        }

        public IActionResult OnGetLogout()
        {
            //HttpContext.SignOutAsync();

            return SignOut(new AuthenticationProperties
            {
                RedirectUri = "/"
            },
            CookieAuthenticationDefaults.AuthenticationScheme,
            OpenIdConnectDefaults.AuthenticationScheme);
        }

    }
}
