using Microsoft.AspNetCore.Mvc;
using StoreFront.Application.Services;
using StoreFront.Domain.Models.FeedbackModels.Response;

namespace StoreFront.Pages.Main.ProductPage.Components.FeedbackProduct
{
    public class FeedbackProductViewComponent : ViewComponent
    {
        private readonly IFeedbackService _feedbackService;
        private readonly SafeApiCaller _safeApiCaller;

        public FeedbackProductViewComponent(IFeedbackService feedbackService, SafeApiCaller safeApiCaller)
        {
            _feedbackService = feedbackService;
            _safeApiCaller = safeApiCaller;
        }

        public async Task<IViewComponentResult> InvokeAsync(Guid productId)
        {
            var feedback = await _safeApiCaller.CallSafeAsync<FeedbackResponse?>(
                () => _feedbackService.GetMyFeedback(productId)
            );

            TempData["ProductId"] = productId;

            return View(feedback.Data);
        }
    }
}
