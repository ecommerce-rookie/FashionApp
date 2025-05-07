using StoreFront.Domain.Models.UserModels.Responses;

namespace StoreFront.Domain.Models.FeedbackModels.Response
{
    public class FeedbackResponse
    {
        public Guid Id { get; set; }
        public AuthorResponse Author { get; set; } = null!;
        public string Content { get; set; } = null!;
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
