using System;
using System.Collections.Generic;

namespace Persistence.Entity;

public partial class Order
{
    public Guid Id { get; set; }

    public long? TotalPrice { get; set; }

    public string? Address { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? OrderStatus { get; set; }

    public string? PaymentMethod { get; set; }

    public string? NameReceiver { get; set; }

    public Guid? CustomerId { get; set; }

    public virtual User? Customer { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<OrderTracking> OrderTrackings { get; set; } = new List<OrderTracking>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
