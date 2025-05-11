using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Aggregates.ProductAggregate.Enums;
using Domain.Exceptions;
using FluentAssertions;

namespace Application.UnitTest.Domain.AggregateTest
{
    public class ProductAggregateTest
    {
        [Fact]
        public void Create_ValidInput_ShouldCreateProduct()
        {
            var product = Product.Create(Guid.NewGuid(), "Shoes", 100, 50, "desc", ProductStatus.Available, 1, 5, new List<string> { "M", "L" }, Gender.Male, Guid.NewGuid());

            product.Name.Should().Be("Shoes");
            product.Price.Should().NotBeNull();
            product.Quantity.Should().Be(5);
            product.Slug.Should().Contain("shoes-");
        }

        [Theory]
        [InlineData("", "Name must be at least 2 characters")]
        [InlineData("A", "Name must be at least 2 characters")]
        public void Create_InvalidName_ShouldThrow(string name, string expectedMessage)
        {
            var action = () => Product.Create(Guid.NewGuid(), name, 100, 50, null, ProductStatus.Available, 1, 5, new List<string>(), Gender.Male, Guid.NewGuid());
            action.Should().Throw<ValidationException>().WithMessage("One or more validation failures have occurred.");
        }

        [Fact]
        public void Create_NegativeUnitPrice_ShouldThrow()
        {
            var action = () => Product.Create(Guid.NewGuid(), "Shoe", -1, 0, null, ProductStatus.Available, 1, 5, new List<string>(), Gender.Male, Guid.NewGuid());
            action.Should().Throw<ValidationException>().WithMessage("One or more validation failures have occurred.");
        }

        [Fact]
        public void Create_PurchasePriceGreaterThanUnitPrice_ShouldThrow()
        {
            var action = () => Product.Create(Guid.NewGuid(), "Shoe", 50, 100, null, ProductStatus.Available, 1, 5, new List<string>(), Gender.Male, Guid.NewGuid());
            action.Should().Throw<ValidationException>().WithMessage("One or more validation failures have occurred.");
        }

        [Fact]
        public void Update_ShouldUpdateProductValues()
        {
            var id = Guid.NewGuid();
            var product = Product.Create(id, "Old", 50, 25, "desc", ProductStatus.Available, 1, 5, new List<string>(), Gender.Male, Guid.NewGuid());

            product.Update(id, "New", 70, 35, "new desc", ProductStatus.Available, 2, 10, new List<string> { "XL" }, Gender.Female);

            product.Name.Should().Be("New");
            product.Price!.UnitPrice.Amount.Should().Be(70);
            product.Quantity.Should().Be(10);
            product.Slug.Should().Contain("new-");
        }

        [Fact]
        public void Update_ZeroQuantityWithAvailableStatus_ShouldBeOutOfStock()
        {
            var id = Guid.NewGuid();
            var product = Product.Create(id, "Shoe", 50, 25, "desc", ProductStatus.Available, 1, 1, new List<string>(), Gender.Male, Guid.NewGuid());

            product.Update(id, "Shoe", 50, 25, "desc", ProductStatus.Available, 1, 0, new List<string>(), Gender.Male);

            product.Status.Should().Be(ProductStatus.OutOfStock);
        }

        [Fact]
        public void Delete_ShouldSetIsDeletedToTrue()
        {
            var product = Product.Create(Guid.NewGuid(), "Item", 10, 5, "desc", ProductStatus.Available, 1, 1, new List<string>(), Gender.Male, Guid.NewGuid());

            product.Delete();

            product.IsDeleted.Should().BeTrue();
        }

    }
}
