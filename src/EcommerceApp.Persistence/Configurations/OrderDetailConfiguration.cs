using Domain.Aggregates.OrderAggregate.Entities;
using Domain.Aggregates.ProductAggregate.ValuesObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public sealed class OrderDetailConfiguration : IEntityTypeConfiguration<OrderDetail>
    {
        public void Configure(EntityTypeBuilder<OrderDetail> builder)
        {
            builder.ToTable(nameof(OrderDetail));

            builder.HasKey(od => od.Id);

            builder.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(e => e.OrderId)
                .HasColumnName("orderId");

            builder.Property(e => e.Price)
                .HasConversion(
                    v => v!.Amount,
                    v => Money.Create(v)
                )
                .HasColumnName("price");

            builder.Property(e => e.ProductId)
                .HasColumnName("productId");

            builder.Property(e => e.Quantity)
                .HasColumnName("quantity");

            builder.Property(d => d.Size)
                .HasColumnName("size");

            builder.HasOne(d => d.Order)
                   .WithMany(p => p.OrderDetails)
                   .HasForeignKey(od => od.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(d => d.Product)
                   .WithMany(p => p.OrderDetails)
                   .HasForeignKey(od => od.ProductId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }

}
