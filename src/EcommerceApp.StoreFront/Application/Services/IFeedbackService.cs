using Refit;
using StoreFront.Domain.Models.Common;
using StoreFront.Domain.Models.FeedbackModels.Request;
using StoreFront.Domain.Models.FeedbackModels.Response;

namespace StoreFront.Application.Services
{
    public interface IFeedbackService
    {
        [Get("/feedbacks/{productId}")]
        Task<ApiResponse<IEnumerable<FeedbackResponse>>> GetFeedbacks(Guid productId, int page, int eachPage);

        [Post("/feedbacks/{productId}")]
        Task<ApiResponse<APIResponse>> CreateFeedback(Guid productId, [Body] FeedbackRequest request);

        [Patch("/feedbacks/{productId}")]
        Task<ApiResponse<APIResponse>> UpdateFeedback(Guid productId, [Body] FeedbackRequest request);

        [Delete("/feedbacks/{feedbackId}")]
        Task<ApiResponse<APIResponse>> DeleteFeedback(Guid feedbackId, [Query] bool? isHard);

        [Get("/feedbacks/{productId}/me")]
        Task<ApiResponse<APIResponse<FeedbackResponse?>>> GetMyFeedback(Guid productId);
    }
}
