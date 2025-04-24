using Application.Features.UserFeatures.Models;

namespace Application.Features.FeedbackFeatures.Models
{
    public class FeedbackResponseModel
    {
        public Guid Id { get; set; }
        public AuthorResponseModel Author { get; set; } = null!;
        public string Content { get; set; } = null!;
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
