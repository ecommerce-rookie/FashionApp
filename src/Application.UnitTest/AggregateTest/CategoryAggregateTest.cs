using Domain.Aggregates.CategoryAggregate.Entities;
using Domain.Exceptions;
using FluentAssertions;

namespace Application.UnitTest.AggregateTest
{
    public class CategoryAggregateTest
    {
        [Fact]
        public void Constructor_WithValidName_ShouldSetName()
        {
            var category = new Category("Clothes");

            category.Name.Should().Be("Clothes");
        }

        [Fact]
        public void Create_WithValidName_ShouldReturnCategory()
        {
            var category = Category.Create("Shoes");

            category.Should().NotBeNull();
            category.Name.Should().Be("Shoes");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WithInvalidName_ShouldThrow(string? invalidName)
        {
            var cat = () => Category.Create(invalidName!);

            cat.Should().Throw<ValidationException>()
                    .WithMessage("One or more validation failures have occurred.");
        }

        [Fact]
        public void Update_ShouldChangeName()
        {
            var category = new Category("Old Name");

            category.Update("New Name");

            category.Name.Should().Be("New Name");
        }

        [Fact]
        public void Update_WithEmptyName_ShouldAllowIfLogicPermits()
        {
            var category = new Category("Initial");
            category.Update("");

            category.Name.Should().Be("");
        }

        [Fact]
        public void Products_ShouldBeEmptyByDefault()
        {
            var category = new Category("Empty Category");

            category.Products.Should().BeEmpty();
        }
    }
}
