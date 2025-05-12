using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.SeedWorks.Implements;
using System.Diagnostics.CodeAnalysis;

namespace Persistence.Configurations;

[ExcludeFromCodeCoverage]
public abstract class BaseConfiguration<T, TKey> : IEntityTypeConfiguration<T>
    where T : BaseAuditableEntity<TKey>
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.Property(e => e.CreatedAt)
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("createdAt");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp with time zone")
            .HasColumnName("updatedAt");

        builder.Property(e => e.Version)
            .HasDefaultValueSql("gen_random_uuid()")
            .HasColumnName("version")
            .IsConcurrencyToken();
    }
}