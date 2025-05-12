using Domain.Aggregates.ProductAggregate.ValuesObjects;
using Domain.Aggregates.UserAggregate.Entities;
using Domain.Aggregates.UserAggregate.Enums;
using Domain.Aggregates.UserAggregate.ValuesObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace Persistence.Configurations
{
    [ExcludeFromCodeCoverage]
    public sealed class UserConfiguration : BaseConfiguration<User, Guid>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            base.Configure(builder);

            builder.HasKey(e => e.Id);

            builder.ToTable(nameof(User));

            builder.Property(e => e.Id)
                .ValueGeneratedNever().HasColumnName("id");

            builder.Property(e => e.Avatar)
                .HasConversion(
                    v => v != null ? v.Url : string.Empty,
                    v => v != null ? ImageUrl.Create(v) : null
                )
                .HasColumnType("character varying")
                .HasColumnName("avatar");

            builder.Property(e => e.Email)
                .HasConversion(
                    v => v.Value, 
                    v => new EmailAddress(v)
                )
                .HasMaxLength(50)
                .HasColumnName("email");

            builder.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("firstName");

            builder.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("lastName");

            builder.Property(e => e.Phone)
                .HasMaxLength(12)
                .HasColumnName("phone");

            builder.Property(e => e.Status)
                .HasMaxLength(50)
                .HasConversion<string>()
                .HasColumnName("status");

            builder.Property(e => e.Role)
                .HasMaxLength(50)
                .HasConversion<string>()
                .HasColumnName("role");

            builder.Property(e => e.IsDeleted)
                .HasColumnName("isDeleted");
        }
    }
}
