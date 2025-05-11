using Domain.Aggregates.ProductAggregate.ValueObjects;
using Domain.Exceptions;
using FluentAssertions;

namespace Application.UnitTest.Domain.ValuesObjectTest
{
    public class ProductPriceTest
    {
        [Fact]
        public void Create_WithValidPrices_ShouldCreateSuccessfully()
        {
            // Arrange
            decimal unitPrice = 1000;
            decimal purchasePrice = 800;

            // Act
            var price = ProductPrice.Create(unitPrice, purchasePrice);

            // Assert
            price.UnitPrice.Amount.Should().Be(unitPrice);
            price.PurchasePrice.Amount.Should().Be(purchasePrice);
        }

        [Fact]
        public void Create_WithNullPurchasePrice_ShouldSetPurchasePriceEqualsUnitPrice()
        {
            // Arrange
            decimal unitPrice = 1200;
            decimal? purchasePrice = null;

            // Act
            var price = new ProductPrice(unitPrice, purchasePrice);

            // Assert
            price.UnitPrice.Amount.Should().Be(unitPrice);
            price.PurchasePrice.Amount.Should().Be(unitPrice);
        }

        [Theory]
        [InlineData(-1, 500)] // UnitPrice âm
        [InlineData(500, -1)] // PurchasePrice âm
        public void Create_WithNegativePrices_ShouldThrowValidationException(decimal unitPrice, decimal purchasePrice)
        {
            // Act
            var act = () => ProductPrice.Create(unitPrice, purchasePrice);

            // Assert
            act.Should().Throw<ValidationException>();
        }

        [Fact]
        public void Create_WithPurchasePriceGreaterThanUnitPrice_ShouldThrowValidationException()
        {
            // Arrange
            decimal unitPrice = 500;
            decimal purchasePrice = 600;

            // Act
            var act = () => ProductPrice.Create(unitPrice, purchasePrice);

            // Assert
            act.Should().Throw<ValidationException>()
                .WithMessage("One or more validation failures have occurred.");
        }

        [Fact]
        public void Equals_SameUnitAndPurchasePrice_ShouldReturnTrue()
        {
            // Arrange
            var price1 = ProductPrice.Create(1000, 800);
            var price2 = ProductPrice.Create(1000, 800);

            // Act & Assert
            price1.Should().Be(price2);
            price1.Equals(price2).Should().BeTrue();
        }

        [Fact]
        public void Equals_DifferentUnitOrPurchasePrice_ShouldReturnFalse()
        {
            // Arrange
            var price1 = ProductPrice.Create(1000, 800);
            var price2 = ProductPrice.Create(1000, 700); // khác purchasePrice

            // Act & Assert
            price1.Should().NotBe(price2);
        }

        [Fact]
        public void GetHashCode_SameValues_ShouldReturnSameHash()
        {
            // Arrange
            var price1 = ProductPrice.Create(1000, 800);
            var price2 = ProductPrice.Create(1000, 800);

            // Act & Assert
            price1.GetHashCode().Should().Be(price2.GetHashCode());
        }
    }
}
