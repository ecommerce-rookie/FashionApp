using Domain.Aggregates.OrderAggregate.Entities;
using Domain.Aggregates.ProductAggregate.ValuesObjects;
using FluentAssertions;

namespace Application.UnitTest.Domain.AggregateTest
{
    public class OrderDetailTest
    {
        [Fact]
        public void Constructor_WithSize_ShouldSetPropertiesCorrectly()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var quantity = 2;
            var price = new Money(100);
            var productId = Guid.NewGuid();
            var size = "M";

            // Act
            var orderDetail = new OrderDetail(orderId, quantity, price, productId, size);

            // Assert
            orderDetail.OrderId.Should().Be(orderId);
            orderDetail.Quantity.Should().Be(quantity);
            orderDetail.Price.Should().Be(price);
            orderDetail.ProductId.Should().Be(productId);
            orderDetail.Size.Should().Be(size);
        }

        [Fact]
        public void Constructor_WithoutSize_ShouldSetPropertiesCorrectly()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var quantity = 1;
            var price = new Money(200);
            var productId = Guid.NewGuid();

            // Act
            var orderDetail = new OrderDetail(orderId, quantity, price, productId);

            // Assert
            orderDetail.OrderId.Should().Be(orderId);
            orderDetail.Quantity.Should().Be(quantity);
            orderDetail.Price.Should().Be(price);
            orderDetail.ProductId.Should().Be(productId);
            orderDetail.Size.Should().BeNull();
        }

        [Fact]
        public void Create_WithSize_ShouldReturnCorrectInstance()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var quantity = 3;
            var price = new Money(300);
            var productId = Guid.NewGuid();
            var size = "L";

            // Act
            var result = OrderDetail.Create(orderId, quantity, price, productId, size);

            // Assert
            result.Should().NotBeNull();
            result.OrderId.Should().Be(orderId);
            result.Quantity.Should().Be(quantity);
            result.Price.Should().Be(price);
            result.ProductId.Should().Be(productId);
            result.Size.Should().Be(size);
        }

        [Fact]
        public void Create_WithoutSize_ShouldReturnCorrectInstance()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var quantity = 5;
            var price = new Money(500);
            var productId = Guid.NewGuid();

            // Act
            var result = OrderDetail.Create(orderId, quantity, price, productId);

            // Assert
            result.Should().NotBeNull();
            result.OrderId.Should().Be(orderId);
            result.Quantity.Should().Be(quantity);
            result.Price.Should().Be(price);
            result.ProductId.Should().Be(productId);
            result.Size.Should().BeNull();
        }

        [Fact]
        public void Update_ShouldUpdateAllFieldsCorrectly()
        {
            // Arrange
            var orderDetail = new OrderDetail(Guid.NewGuid(), 1, new Money(50), Guid.NewGuid(), "S");
            var newQuantity = 10;
            var newPrice = new Money(999);
            var newProductId = Guid.NewGuid();
            var newSize = "XL";

            // Act
            orderDetail.Update(newQuantity, newPrice, newProductId, newSize);

            // Assert
            orderDetail.Quantity.Should().Be(newQuantity);
            orderDetail.Price.Should().Be(newPrice);
            orderDetail.ProductId.Should().Be(newProductId);
            orderDetail.Size.Should().Be(newSize);
        }
    }
}
