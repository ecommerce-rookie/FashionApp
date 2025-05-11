using Domain.Aggregates.OrderAggregate.Entities;
using Domain.Aggregates.OrderAggregate.Enums;
using Domain.Aggregates.OrderAggregate.ValuesObject;
using FluentAssertions;

namespace Application.UnitTest.Domain.AggregateTest
{
    public class OrderAggregateTest
    {
        public class OrderTests
        {
            [Fact]
            public void Create_ShouldReturnOrder_WhenValidParameters()
            {
                // Arrange
                var totalPrice = 100m;
                var address = "123 Main St";
                var status = OrderStatus.Pending;
                var method = PaymentMethod.Cash;
                var nameReceiver = "John Doe";
                var customerId = Guid.NewGuid();

                // Act
                var order = Order.Create(totalPrice, address, status, method, nameReceiver, customerId);

                // Assert
                order.Should().NotBeNull();
                order.TotalPrice.Amount.Should().Be(totalPrice);
                order.Address.Should().Be(address);
                order.OrderStatus.Should().Be(status);
                order.PaymentMethod.Should().Be(method);
                order.NameReceiver.Should().Be(nameReceiver);
                order.CustomerId.Should().Be(customerId);
            }

            [Theory]
            [InlineData(null)]
            [InlineData("")]
            public void Create_ShouldThrow_WhenAddressInvalid(string invalidAddress)
            {
                // Arrange
                var customerId = Guid.NewGuid();

                // Act
                Action act = () => Order.Create(100m, invalidAddress, OrderStatus.Pending, PaymentMethod.Cash, "Receiver", customerId);

                // Assert
                act.Should().Throw<ArgumentException>().WithMessage("*Address cannot be null or empty*");
            }

            [Fact]
            public void Create_ShouldThrow_WhenTotalPriceNegative()
            {
                // Act
                Action act = () => Order.Create(-5, "Address", OrderStatus.Pending, PaymentMethod.Cash, "Receiver", Guid.NewGuid());

                // Assert
                act.Should().Throw<ArgumentException>().WithMessage("*Total price cannot be negative*");
            }

            [Theory]
            [InlineData(null)]
            [InlineData("")]
            public void Create_ShouldThrow_WhenReceiverInvalid(string receiver)
            {
                Action act = () => Order.Create(100, "Address", OrderStatus.Pending, PaymentMethod.Cash, receiver, Guid.NewGuid());

                act.Should().Throw<ArgumentException>().WithMessage("*Name receiver cannot be null or empty*");
            }

            [Fact]
            public void Create_ShouldThrow_WhenCustomerIdIsEmpty()
            {
                Action act = () => Order.Create(100, "Address", OrderStatus.Pending, PaymentMethod.Cash, "Receiver", Guid.Empty);

                act.Should().Throw<ArgumentException>().WithMessage("*Customer ID cannot be empty*");
            }

            [Fact]
            public void UpdateTotalPrice_ShouldUpdateValue_WhenValid()
            {
                var order = CreateSampleOrder();

                order.UpdateTotalPrice(300);

                order.TotalPrice.Amount.Should().Be(300);
            }

            [Fact]
            public void UpdateTotalPrice_ShouldThrow_WhenNegative()
            {
                var order = CreateSampleOrder();

                Action act = () => order.UpdateTotalPrice(-100);

                act.Should().Throw<ArgumentException>().WithMessage("*Total price cannot be negative*");
            }

            [Fact]
            public void UpdateStatus_ShouldUpdateOrderStatus()
            {
                var order = CreateSampleOrder();

                order.UpdateStatus(OrderStatus.Shipping);

                order.OrderStatus.Should().Be(OrderStatus.Shipping);
            }

            [Fact]
            public void CreateOrderDetail_ShouldAddOrderDetails()
            {
                var order = CreateSampleOrder();
                var carts = new List<Cart>
                {
                    new Cart(Guid.NewGuid(), 2, 100),
                    new Cart(Guid.NewGuid(), 1, 200)
                };

                order.CreateOrderDetail(carts);

                order.OrderDetails.Should().NotBeNull();
                order.OrderDetails.Should().HaveCount(2);
                order.OrderDetails.First().Quantity.Should().Be(2);
                order.OrderDetails.Last().Price!.Amount.Should().Be(200);
            }

            private Order CreateSampleOrder()
            {
                return Order.Create(100, "123 Main", OrderStatus.Pending, PaymentMethod.Cash, "Jane", Guid.NewGuid());
            }
        }

    }
}
