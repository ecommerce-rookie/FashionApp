using System;
using System.Collections.Generic;

namespace Persistence.Entity;

public partial class OrderTracking
{
    public int Id { get; set; }

    public string? OrderStatus { get; set; }

    public Guid? OrderId { get; set; }

    public string? Note { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Order? Order { get; set; }
}
