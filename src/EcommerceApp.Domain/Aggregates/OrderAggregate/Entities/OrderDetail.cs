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

    public virtual Order? Order { get; set; }

    public virtual Product? Product { get; set; }

    public OrderDetail() { }

    public OrderDetail(Guid? orderId, int? quantity, Money? price, Guid? productId, string? size)
    {
        OrderId = orderId;
        Quantity = quantity;
        Price = price;
        ProductId = productId;
        Size = size;
    }

    public OrderDetail(Guid? orderId, int? quantity, Money? price, Guid? productId)
    {
        OrderId = orderId;
        Quantity = quantity;
        Price = price;
        ProductId = productId;
    }

    public static OrderDetail Create(Guid? orderId, int? quantity, Money? price, Guid? productId, string? size)
    {
        return new OrderDetail(orderId, quantity, price, productId, size);
    }

    public static OrderDetail Create(Guid? orderId, int? quantity, Money? price, Guid? productId)
    {
        return new OrderDetail(orderId, quantity, price, productId);
    }

    public void Update(int quantity, Money price, Guid productId, string size)
    {
        Quantity = quantity;
        Price = price;
        ProductId = productId;
        Size = size;
    }

}
