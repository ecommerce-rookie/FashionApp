using Microsoft.AspNetCore.Mvc;
using StoreFront.Application.Helpers;
using StoreFront.Application.Services;
using StoreFront.Domain.Constants;

namespace StoreFront.Pages.Main.ProductPage.Components.ReviewProduct
{
    public class ReviewProductViewComponent : ViewComponent
    {
        private readonly IFeedbackService _feedbackService;

        public ReviewProductViewComponent(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        public async Task<IViewComponentResult> InvokeAsync(Guid productId)
        {
            var response = await _feedbackService.GetFeedbacks(productId, DefaultConstant.Page, DefaultConstant.ReviewProductCount);

            var reviews = response.ToPagedList();

            return View(reviews);
        }

    }
}
