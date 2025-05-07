using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Aggregates.ProductAggregate.ValuesObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public sealed class ImageProductConfiguration : IEntityTypeConfiguration<ImageProduct>
    {
        public void Configure(EntityTypeBuilder<ImageProduct> builder)
        {
            builder.HasKey(ip => new { ip.Image, ip.ProductId });
            
            builder.ToTable(nameof(ImageProduct));

            builder.Property(e => e.ProductId)
                .HasColumnName("productId");

            builder.Property(ip => ip.Image)
                    .HasConversion(
                        v => v != null ? v.Url : string.Empty,
                        v => ImageUrl.Create(v)
                    )
                    .HasColumnType("character varying")
                    .HasColumnName("image")
                    .IsRequired();

            builder.Property(e => e.OrderNumber)
                .HasColumnName("orderNumber");

            builder.HasOne(d => d.Product)
                   .WithMany(d => d.ImageProducts)
                   .HasForeignKey(ip => ip.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
