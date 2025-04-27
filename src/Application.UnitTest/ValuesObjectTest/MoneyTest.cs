using Domain.Aggregates.ProductAggregate.Enums;
using Domain.Aggregates.ProductAggregate.ValuesObjects;
using Domain.Exceptions;
using FluentAssertions;

namespace Application.UnitTest.ValuesObjectTest
{
    public class MoneyTest
    {
        [Fact]
        public void Create_WithValidAmount_ShouldCreateSuccessfully()
        {
            // Arrange
            decimal amount = 1000m;

            // Act
            var money = Money.Create(amount);

            // Assert
            money.Amount.Should().Be(amount);
            money.Currency.Should().Be(CurrencyEnum.VND);
        }

        [Fact]
        public void Create_WithValidAmountAndCurrency_ShouldCreateSuccessfully()
        {
            // Arrange
            decimal amount = 500m;
            var currency = CurrencyEnum.USD;

            // Act
            var money = Money.Create(amount, currency);

            // Assert
            money.Amount.Should().Be(amount);
            money.Currency.Should().Be(currency);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Create_WithNegativeAmount_ShouldThrowValidationException(decimal invalidAmount)
        {
            // Act
            var act = () => Money.Create(invalidAmount);

            // Assert
            act.Should().Throw<ValidationException>()
                .WithMessage("One or more validation failures have occurred.");
        }

        [Fact]
        public void Equals_SameAmountAndCurrency_ShouldReturnTrue()
        {
            // Arrange
            var money1 = Money.Create(1000, CurrencyEnum.VND);
            var money2 = Money.Create(1000, CurrencyEnum.VND);

            // Act & Assert
            money1.Should().Be(money2);
            money1.Equals(money2).Should().BeTrue();
        }

        [Fact]
        public void Equals_DifferentAmountOrCurrency_ShouldReturnFalse()
        {
            // Arrange
            var money1 = Money.Create(1000, CurrencyEnum.VND);
            var money2 = Money.Create(2000, CurrencyEnum.VND);
            var money3 = Money.Create(1000, CurrencyEnum.USD);

            // Act & Assert
            money1.Should().NotBe(money2);
            money1.Should().NotBe(money3);
        }

        [Fact]
        public void GetHashCode_SameObject_ShouldBeConsistent()
        {
            // Arrange
            var money1 = Money.Create(500, CurrencyEnum.VND);
            var money2 = Money.Create(500, CurrencyEnum.VND);

            // Act & Assert
            money1.GetHashCode().Should().Be(money2.GetHashCode());
        }

        [Fact]
        public void DefaultConstructor_ShouldInitializeWithDefaults()
        {
            // Act
            var money = new Money();

            // Assert
            money.Amount.Should().Be(0);
            money.Currency.Should().Be(CurrencyEnum.VND);
        }
    }
}
