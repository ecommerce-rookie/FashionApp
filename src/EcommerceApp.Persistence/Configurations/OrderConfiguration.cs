using Domain.Aggregates.OrderAggregate.Entities;
using Domain.Aggregates.OrderAggregate.Enums;
using Domain.Aggregates.ProductAggregate.ValuesObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public sealed class OrderConfiguration : BaseConfiguration<Order, Guid>
    {
        public override void Configure(EntityTypeBuilder<Order> builder)
        {
            base.Configure(builder);

            builder.ToTable(nameof(Order));

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id)
                .HasColumnName("id")
                .ValueGeneratedNever();

            builder.Property(e => e.Address)
                .HasColumnName("address");

            builder.Property(e => e.CustomerId)
                .HasColumnName("customerId");

            builder.Property(e => e.NameReceiver)
                .HasMaxLength(100)
                .HasColumnName("nameReceiver");

            builder.Property(e => e.PaymentMethod)
                .HasConversion<string>()
                .HasColumnName("paymentMethod");
;
            builder.Property(e => e.TotalPrice)
                .HasConversion(
                    v => v.Amount,
                    v => Money.Create(v)
                )
                .HasColumnName("totalPrice");

            builder.Property(e => e.OrderStatus)
                .HasConversion<string>()
                .HasColumnName("orderStatus");

            builder.HasOne(d => d.Customer)
                   .WithMany(p => p.Orders)
                   .HasForeignKey(o => o.CustomerId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }

}
