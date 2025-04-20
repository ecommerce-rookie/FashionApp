using Domain.Aggregates.ProductAggregate.ValuesObjects;
using Domain.Aggregates.UserAggregate.Enums;
using Domain.Aggregates.UserAggregate.ValuesObject;

namespace Application.Features.UserFeatures.Models
{
    public class UserPreviewResponseModel
    {
        public string Email { get; set; } = null!;

        public string? FirstName { get; set; }

        public string LastName { get; set; } = null!;

        public string? Phone { get; set; }

        public string? Avatar { get; set; }

        public string? Status { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
