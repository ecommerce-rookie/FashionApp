using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using StoreFront.Application.Extensions;
using StoreFront.Application.Helpers;
using StoreFront.Application.Services.CartService;

namespace StoreFront.Pages.Main.HomePage
{
    public class HomePageModel : PageModel
    {
        private readonly ILogger<HomePageModel> _logger;
        private readonly ICartService _cartService;

        public HomePageModel(ILogger<HomePageModel> logger, ICartService cartService)
        {;
            _logger = logger;
            _cartService = cartService;
        }

        public async Task OnGet(string? type, string? action)
        {
            if(action?.Equals("login") ?? false)
            {
                TempData.SetSuccess("Welcome back!", "Login Success!");
            } else if(action?.Equals("logout") ?? false)
            {
                TempData.SetSuccess("See you next time!", "Logout Success!");
            }


        }

        

    }
}
