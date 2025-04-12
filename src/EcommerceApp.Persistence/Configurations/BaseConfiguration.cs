using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.SeedWorks.Implements;

namespace Persistence.Configurations;

public abstract class BaseConfiguration<T, TKey> : IEntityTypeConfiguration<T>
    where T : BaseAuditableEntity<TKey>
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.Property(e => e.CreatedAt)
            .HasDefaultValue(DateTime.UtcNow);

        builder.Property(e => e.UpdatedAt)
            .HasDefaultValue(DateTime.UtcNow);

        builder.Property(e => e.Version)
            .HasDefaultValue(Guid.NewGuid())
            .IsConcurrencyToken();
    }
}