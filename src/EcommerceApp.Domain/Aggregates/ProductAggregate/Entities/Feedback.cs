using Domain.Aggregates.UserAggregate.Entities;
using Domain.SeedWorks.Events;
using Persistence.SeedWorks.Abstractions;

namespace Domain.Aggregates.ProductAggregate.Entities
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

        public void Update(string content, int rating)
        {
            Content = content;
            Rating = rating;
        }

        public void Delete()
        {
            IsDeleted = true;
        }

    }
}
