using Domain.Aggregates.FeedbackAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public sealed class FeedbackConfiguration : BaseConfiguration<Feedback, Guid>
    {
        public override void Configure(EntityTypeBuilder<Feedback> builder)
        {
            base.Configure(builder);

            builder.HasKey(e => e.Id);

            builder.ToTable(nameof(Feedback));

            builder.Property(e => e.Id)
                .ValueGeneratedNever().HasColumnName("id");

            builder.Property(e => e.Content)
                .HasColumnName("content");

            builder.Property(e => e.IsDeleted)
                .HasColumnName("isDeleted");

            builder.Property(e => e.ProductId)
                .HasColumnName("productId");

            builder.Property(e => e.UserId)
                .HasColumnName("userId");

            builder.Property(e => e.Rating)
                .HasColumnName("rating");

            builder.HasOne(d => d.CreatedByNavigation)
                   .WithMany(d => d.Feedbacks)
                   .HasForeignKey(p => p.UserId)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(d => d.Product)
                     .WithMany(d => d.Feedbacks)
                     .HasForeignKey(p => p.ProductId)
                     .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
