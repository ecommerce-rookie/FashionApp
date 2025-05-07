using Domain.Aggregates.OrderAggregate.Enums;
using Domain.Aggregates.OrderAggregate.ValuesObject;
using Domain.Aggregates.ProductAggregate.ValuesObjects;
using Domain.Aggregates.UserAggregate.Entities;
using Domain.SeedWorks.Abstractions;
using Persistence.SeedWorks.Implements;

namespace Domain.Aggregates.OrderAggregate.Entities;

public partial class Order : BaseAuditableEntity<Guid>, IAggregateRoot
{
    public Money TotalPrice { get; private set; } = new Money(0);

    public string? Address { get; private set; }

    public OrderStatus? OrderStatus { get; private set; }

    public PaymentMethod? PaymentMethod { get; private set; }

    public string? NameReceiver { get; private set; }

    public Guid? CustomerId { get; private set; }

    public virtual User? Customer { get; private set; }

    public virtual ICollection<OrderDetail>? OrderDetails { get; private set; } = new List<OrderDetail>();


    public Order() { }

    public Order(decimal totalPrice, string address, OrderStatus status, PaymentMethod method, 
        string nameReceiver, Guid customerId)
    {
        Id = Guid.NewGuid();
        TotalPrice = new Money(totalPrice);
        Address = address;
        OrderStatus = status;
        PaymentMethod = method;
        NameReceiver = nameReceiver;
        CustomerId = customerId;
    }

    public static Order Create(decimal totalPrice, string address, OrderStatus status, PaymentMethod method,
        string nameReceiver, Guid customerId)
    {
        if(string.IsNullOrWhiteSpace(address))
        {
            throw new ArgumentException("Address cannot be null or empty.", nameof(address));
        }

        if (totalPrice < 0)
        {
            throw new ArgumentException("Total price cannot be negative.", nameof(totalPrice));
        }

        if (string.IsNullOrWhiteSpace(nameReceiver))
        {
            throw new ArgumentException("Name receiver cannot be null or empty.", nameof(nameReceiver));
        }

        if (customerId == Guid.Empty)
        {
            throw new ArgumentException("Customer ID cannot be empty.", nameof(customerId));
        }

        return new Order(totalPrice, address, status, method, nameReceiver, customerId);
    }

    public void CreateOrderDetail(IEnumerable<Cart> carts)
    {
        if(OrderDetails == null)
        {
            OrderDetails = new List<OrderDetail>();
        }

        foreach (var cart in carts)
        {
            var orderDetail = OrderDetail.Create(this.Id, cart.Quantity, cart.Price, cart.ProductId);

            OrderDetails.Add(orderDetail);
        }
    }

    public void UpdateStatus(OrderStatus status)
    {
        OrderStatus = status;
    }

    public void UpdateTotalPrice(decimal totalPrice)
    {
        if (totalPrice < 0)
        {
            throw new ArgumentException("Total price cannot be negative.", nameof(totalPrice));
        }
     
        TotalPrice = new Money(totalPrice);
    }

}
