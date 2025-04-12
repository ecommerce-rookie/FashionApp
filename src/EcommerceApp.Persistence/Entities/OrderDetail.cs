using System;
using System.Collections.Generic;

namespace Persistence.Entity;

public partial class OrderDetail
{
    public int Id { get; set; }

    public Guid? OrderId { get; set; }

    public int? Quantity { get; set; }

    public long? Price { get; set; }

    public Guid? ProductId { get; set; }

    public string? Size { get; set; }

    public string? Color { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Product? Product { get; set; }
}
