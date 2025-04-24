using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Aggregates.ProductAggregate.ValuesObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public sealed class ProductConfiguration : BaseConfiguration<Product, Guid>
    {
        public override void Configure(EntityTypeBuilder<Product> builder)
        {
            base.Configure(builder);

            builder.ToTable(nameof(Product));

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");

            builder.Property(p => p.Name)
                   .IsRequired()
                   .HasColumnName("name");

            builder.Property(p => p.CategoryId)
                     .HasColumnName("categoryId");

            builder.Property(p => p.Slug)
                     .HasColumnName("slug");

            builder.OwnsOne(p => p.Price, price =>
            {
                price.Property(p => p.UnitPrice)
                     .HasConversion(
                         v => v.Amount,
                         v => Money.Create(v)
                     )
                     .HasColumnName("unitPrice")
                     .IsRequired();

                price.Property(p => p.PurchasePrice)
                    .HasConversion(
                             v => v.Amount,
                             v => Money.Create(v)
                     )
                     .HasColumnName("purchasePrice")
                     .IsRequired();
            });

            builder.Property(p => p.Description)
                     .HasColumnType("character varying")
                     .HasColumnName("description");

            builder.Property(p => p.Status)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasColumnName("status");

            builder.Property(p => p.Sizes)
                    .HasColumnType("jsonb")
                    .HasColumnName("sizes");


            builder.Property(p => p.Gender)
                    .HasConversion<string>()
                    .HasColumnName("gender");

            builder.Property(p => p.Quantity)
                    .HasColumnName("quantity");

            builder.Property(p => p.CreatedBy)
                    .HasColumnName("createdBy");

            builder.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("isDeleted");

            builder.HasOne(d => d.Category)
                   .WithMany(d => d.Products)
                   .HasForeignKey(p => p.CategoryId)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.Products)
                   .HasForeignKey(p => p.CreatedBy)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }

}
