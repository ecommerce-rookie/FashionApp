using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Aggregates.UserAggregate.Entities;
using Domain.Exceptions;
using Domain.SeedWorks.Events;
using Persistence.SeedWorks.Abstractions;

namespace Domain.Aggregates.FeedbackAggregate.Entities
{
    public partial class Feedback : BaseDomainEvents<Guid>, ISoftDelete
    {
        public string Content { get; set; } = null!;

        public Guid? UserId { get; set; }

        public Guid? ProductId { get; set; }

        public virtual User? CreatedByNavigation { get; set; }

        public virtual Product? Product { get; set; }

        public bool IsDeleted { get; set; }

        public int Rating { get; set; }

        public Feedback()
        {
        }

        public Feedback(Guid id, string content, Guid userId, Guid productId, int rating)
        {
            Id = id;
            Content = content;
            IsDeleted = false;
            UserId = userId;
            ProductId = productId;
            Rating = rating;
        }

        public static Feedback Create(string content, Guid userId, Guid productId, int rating)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ValidationException("Content cannot be empty or null", nameof(content));

            if (rating < 1 || rating > 5)
                throw new ValidationException("Rating must be between 1 and 5", nameof(rating));

            return new Feedback(Guid.NewGuid(), content, userId, productId, rating);
        }

        public void Update(string content, int rating)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ValidationException("Content cannot be empty or null", nameof(content));

            if (rating < 1 || rating > 5)
                throw new ValidationException("Rating must be between 1 and 5", nameof(rating));

            Content = content;
            Rating = rating;
        }

        public void Delete()
        {
            IsDeleted = true;
        }

    }
}
