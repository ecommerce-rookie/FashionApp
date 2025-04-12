using System;
using System.Collections.Generic;

namespace Persistence.Entity;

public partial class Product
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public long UnitPrice { get; set; }

    public long PurchasePrice { get; set; }

    public string Description { get; set; } = null!;

    public string Status { get; set; } = null!;

    public int Quantity { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CategoryId { get; set; }

    public Guid? CreatedBy { get; set; }

    public string? Color { get; set; }

    public string? Size { get; set; }

    public string? Gender { get; set; }

    public virtual Category? Category { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<ImageProduct> ImageProducts { get; set; } = new List<ImageProduct>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
