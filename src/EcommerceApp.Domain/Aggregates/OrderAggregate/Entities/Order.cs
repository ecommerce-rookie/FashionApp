using Domain.Aggregates.OrderAggregate.Enums;
using Domain.Aggregates.OrderAggregate.ValuesObject;
using Domain.Aggregates.ProductAggregate.ValuesObjects;
using Domain.Aggregates.UserAggregate.Entities;
using Domain.Aggregates.UserAggregate.ValuesObject;
using Persistence.SeedWorks.Implements;

namespace Domain.Aggregates.OrderAggregate.Entities;

public partial class Order : BaseAuditableEntity<Guid>
{
    public Money? TotalPrice { get; set; }

    public string? Address { get; set; }

    public OrderStatus? OrderStatus { get; set; }

    public PaymentMethod? PaymentMethod { get; set; }

    public string? NameReceiver { get; set; }

    public Guid? CustomerId { get; set; }

    public virtual User? Customer { get; set; }

    public virtual ICollection<OrderDetail>? OrderDetails { get; set; } = new List<OrderDetail>();
}
