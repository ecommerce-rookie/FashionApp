using Domain.Aggregates.OrderAggregate.ValuesObject;
using Domain.Aggregates.ProductAggregate.ValuesObjects;
using Domain.Exceptions;
using FluentAssertions;

namespace Application.UnitTest.Domain.ValuesObjectTest
{
    public class CartTest
    {
        [Fact]
        public void Constructor_WithValidData_ShouldInitializeCorrectly()
        {
            // Arrange
            var productId = Guid.NewGuid();
            int quantity = 3;
            decimal price = 150;

            // Act
            var cart = new Cart(productId, quantity, price);

            // Assert
            cart.ProductId.Should().Be(productId);
            cart.Quantity.Should().Be(quantity);
            cart.Price.Should().Be(new Money(price));
        }

        [Fact]
        public void Constructor_WithInvalidQuantity_ShouldThrowValidationException()
        {
            // Arrange
            var productId = Guid.NewGuid();
            int quantity = 0;
            decimal price = 100;

            // Act
            Action act = () => new Cart(productId, quantity, price);

            // Assert
            act.Should()
                .Throw<ValidationException>()
                .WithMessage("One or more validation failures have occurred.");
        }

        [Fact]
        public void Create_WithValidData_ShouldReturnCartInstance()
        {
            // Arrange
            var productId = Guid.NewGuid();
            int quantity = 2;
            decimal price = 50;

            // Act
            var result = Cart.Create(productId, quantity, price);

            // Assert
            result.Should().NotBeNull();
            result.ProductId.Should().Be(productId);
            result.Quantity.Should().Be(quantity);
            result.Price.Should().Be(new Money(price));
        }

        [Fact]
        public void Equality_TwoIdenticalCarts_ShouldBeEqual()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var cart1 = Cart.Create(productId, 2, 100);
            var cart2 = Cart.Create(productId, 2, 100);

            // Act & Assert
            cart1.Should().Be(cart2);
        }

        [Fact]
        public void Equality_DifferentQuantityOrPrice_ShouldNotBeEqual()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var cart1 = Cart.Create(productId, 2, 100);
            var cart2 = Cart.Create(productId, 3, 100); // different quantity
            var cart3 = Cart.Create(productId, 2, 200); // different price

            // Act & Assert
            cart1.Should().NotBe(cart2);
            cart1.Should().NotBe(cart3);
        }
    }
}
