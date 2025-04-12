using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Entity;

namespace Persistence.Configurations
{
    public sealed class UserConfiguration : BaseConfiguration<User, Guid>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            base.Configure(builder);

            builder.HasKey(u => u.Id);

            builder.Property(e => e.Id)
               .ValueGeneratedNever();

            builder.Property(u => u.Email)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(u => u.LastName)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(u => u.Phone)
                   .HasMaxLength(12);

            builder.Property(u => u.Avatar);

            builder.Property(u => u.Status)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(u => u.FirstName)
                   .HasMaxLength(50);

            builder.Property(u => u.Gender)
                   .HasMaxLength(10);

            builder.Property(u => u.DeletedAt);
        }
    }
}
