using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Aggregates.ProductAggregate.Enums;
using Domain.Exceptions;
using FluentAssertions;

namespace Application.UnitTest.AggregateTest
{
    public class ProductAggregateTest
    {
        #region Create Product
        [Fact]
        public void Create_WithValidParameters_ShouldReturnProduct()
        {
            // Arrange
            var id = Guid.NewGuid();
            var name = "T-Shirt";
            decimal unitPrice = 100;
            decimal purchasePrice = 50;
            var description = "Cool shirt";
            var status = ProductStatus.Available;
            int categoryId = 1;
            int quantity = 10;
            var sizes = new List<string> { "M", "L" };
            var gender = Gender.Male;

            // Act
            var product = Product.Create(id, name, unitPrice, purchasePrice, description, status, categoryId, quantity, sizes, gender, Guid.NewGuid());

            // Assert
            product.Name.Should().Be(name);
            product.Quantity.Should().Be(quantity);
            product.Status.Should().Be(ProductStatus.Available);
            product.Price.Should().NotBeNull();
            product.Slug.Should().Contain(name.ToLower());
        }

        [Fact]
        public void Create_WithInvalidName_ShouldThrow()
        {
            var act = () => Product.Create(Guid.NewGuid(), "A", 100, 50, "", ProductStatus.Available, 1, 1, new(), Gender.Male, Guid.NewGuid());

            act.Should().Throw<ValidationException>().WithMessage("One or more validation failures have occurred.");
        }

        [Theory]
        [InlineData(-1, 0)] // negative unit price
        [InlineData(100, -10)] // negative purchase price
        [InlineData(10, 20)] // purchase > unit
        [InlineData(100, 90, -5)] // negative quantity
        public void Create_WithInvalidValues_ShouldThrow(decimal unitPrice, decimal purchasePrice, int? quantity = 1)
        {
            var act = () => Product.Create(Guid.NewGuid(), "Shirt", unitPrice, purchasePrice, "", ProductStatus.Available, 1, quantity, new(), Gender.Male, Guid.NewGuid());

            act.Should().Throw<ValidationException>();
        }

        #endregion

        #region Update Product

        [Fact]
        public void Update_ShouldUpdateFieldsAndSetSlug()
        {
            var product = Product.Create(Guid.NewGuid(), "Old", 10, 5, "", ProductStatus.OutOfStock, 1, 1, new(), Gender.Male, Guid.NewGuid());

            var newId = Guid.NewGuid();
            product.Update(newId, "New", 50, 20, "updated", ProductStatus.Available, 2, 0, new List<string>() { "S" }, Gender.Female);

            product.Name.Should().Be("New");
            product.Slug.Should().Contain("new");
            product.Status.Should().Be(ProductStatus.OutOfStock); // quantity = 0 -> force status change
        }


        #endregion

    }
}
