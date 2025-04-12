using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Aggregates.ProductAggregate.ValuesObjects;

namespace Domain.Aggregates.OrderAggregate.Entities;

public partial class OrderDetail
{
    public int Id { get; set; }

    public Guid? OrderId { get; set; }

    public int? Quantity { get; set; }

    public Money? Price { get; set; }

    public Guid? ProductId { get; set; }

    public string? Size { get; set; }

    public string? Color { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Product? Product { get; set; }
}
