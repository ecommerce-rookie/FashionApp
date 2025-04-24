using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StoreFront.Application.Helpers;
using StoreFront.Application.Services;
using StoreFront.Domain.Constants;
using StoreFront.Domain.Models.Common;
using StoreFront.Domain.Models.FeedbackModels.Request;
using StoreFront.Domain.Models.ProductModels.Responses;
using System.Net;

namespace StoreFront.Pages.Main.ProductPage
{
    public class IndexModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly IFeedbackService _feedbackService;
        private readonly SafeApiCaller _caller;

        public IndexModel(IProductService productService, IFeedbackService feedbackService,
            SafeApiCaller safeApiCaller)
        {
            _productService = productService;
            _feedbackService = feedbackService;
            _caller = safeApiCaller;
        }

        [FromRoute]
        public string Slug { get; set; } = string.Empty;

        [BindProperty]
        public FeedbackRequest Feedback { get; set; } = new();

        [BindProperty]
        public string Button { get; set; } = string.Empty;

        [BindProperty]
        public Guid ProductId { get; set; } = Guid.Empty;

        [BindProperty]
        public Guid FeedbackId { get; set; } = Guid.Empty;

        public ProductResponseModel Product { get; set; } = new();

        public async Task OnGet()
        {
            if(string.IsNullOrEmpty(Slug))
            {
                RedirectToPage(PageConstants.ShopPage);
            }

            var response = await _productService.GetProduct(Slug);

            if (response.Status != HttpStatusCode.OK || response.Data == null)
            {
                RedirectToPage(PageConstants.ShopPage);
            }

            if (TempData["feedback"] != null && TempData["feedback"]!.Equals("true"))
            {
                TempData.SetSuccess("Thank you!", "Feedback Success!");
            } else if(TempData["feedback"] != null && TempData["feedback"]!.Equals("false"))
            {

            }

            Product = response.Data!;
        }

        public async Task<IActionResult> OnPostFeedbackAsync()
        {
            APIResponse result = new APIResponse() { Status = HttpStatusCode.BadRequest };

            if (Button.Equals("create"))
            {
                result = await _caller.CallSafeAsync(() => _feedbackService.CreateFeedback(ProductId, Feedback));
            } else if(Button.Equals("update"))
            {
                result = await _caller.CallSafeAsync(() => _feedbackService.UpdateFeedback(ProductId, Feedback));
            } else if(Button.Equals("delete"))
            {
                result = await _caller.CallSafeAsync(() => _feedbackService.DeleteFeedback(FeedbackId, true));
            }

            TempData["feedback"] = result.IsSuccess;

            return RedirectToPage();
        }

    }
}
