using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StoreFront.Application.Helpers;
using StoreFront.Application.Services;
using StoreFront.Domain.Constants;
using StoreFront.Domain.Models.PageModels;
using StoreFront.Domain.Models.UserModels.Requests;

namespace StoreFront.Pages.Main.OnBoardingPage
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IUserService _userService;

        public IndexModel(IUserService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        public OnBoardingPageModel OnBoardingPageModel { get; set; } = default!;

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostCreateInforCustomerAsync()
        {
            var address = $"{OnBoardingPageModel.Province}, {OnBoardingPageModel.District}, {OnBoardingPageModel.Ward}, {OnBoardingPageModel.StreetAddress}";
            var result = await _userService.CreateUserInfo(OnBoardingPageModel.FirstName!, OnBoardingPageModel.LastName!,
                OnBoardingPageModel.PhoneNumber!, address, OnBoardingPageModel.Avatar!.CreateStreamPart());

            if(result.IsSuccessStatusCode)
            {
                TempData["Success"] = result.Content?.Message;
                
                return RedirectToPage(PageConstants.HomePage);
            } else
            {
                TempData["Error"] = result.Content?.Message;

                return RedirectToPage(PageConstants.OnboardingPage);
            }

        }

    }
}
