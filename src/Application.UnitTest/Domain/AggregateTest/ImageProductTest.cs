using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Aggregates.ProductAggregate.Enums;
using Domain.Exceptions;
using FluentAssertions;

namespace Application.UnitTest.Domain.AggregateTest
{
    public class ImageProductTest
    {
        [Fact]
        public void Create_ValidInput_ShouldCreateImageProduct()
        {
            var productId = Guid.NewGuid();
            var image = ImageProduct.Create("http://image.com/a.jpg", productId, 1);

            image.Image.Url.Should().Be("http://image.com/a.jpg");
            image.ProductId.Should().Be(productId);
            image.OrderNumber.Should().Be(1);
        }

        [Fact]
        public void Create_EmptyUrl_ShouldThrowValidationException()
        {
            var action = () => ImageProduct.Create("", Guid.NewGuid(), 1);
            action.Should().Throw<ValidationException>().WithMessage("One or more validation failures have occurred.");
        }

        [Fact]
        public void AddImage_ShouldAddToImageProducts()
        {
            var product = Product.Create(Guid.NewGuid(), "Tee", 10, 5, null, ProductStatus.Available, 1, 1, new List<string>(), Gender.Male, Guid.NewGuid());

            product.AddImage("http://img.com/1.jpg", 1);

            product.ImageProducts.Should().ContainSingle(i => i.Image.Url == "http://img.com/1.jpg" && i.OrderNumber == 1);
        }

        [Fact]
        public void DeleteImage_ExistingUrl_ShouldRemoveImage()
        {
            var product = Product.Create(Guid.NewGuid(), "Tee", 10, 5, null, ProductStatus.Available, 1, 1, new List<string>(), Gender.Male, Guid.NewGuid());
            product.AddImage("http://img.com/1.jpg", 1);

            product.DeleteImage("http://img.com/1.jpg");

            product.ImageProducts.Should().BeEmpty();
        }

        [Fact]
        public void DeleteImage_NonExistentUrl_ShouldThrow()
        {
            var product = Product.Create(Guid.NewGuid(), "Tee", 10, 5, null, ProductStatus.Available, 1, 1, new List<string>(), Gender.Male, Guid.NewGuid());
            product.AddImage("http://img.com/1.jpg", 1);
            product.AddImage("http://img.com/2.jpg", 2);

            Action act = () => product.DeleteImage("http://img.com/missing.jpg");
            act.Should().Throw<ValidationException>().WithMessage("One or more validation failures have occurred.");
        }

        [Fact]
        public void DeleteImages_ShouldRemoveAllGiven()
        {
            var product = Product.Create(Guid.NewGuid(), "Tee", 10, 5, null, ProductStatus.Available, 1, 1, new List<string>(), Gender.Male, Guid.NewGuid());
            product.AddImage("http://img.com/1.jpg", 1);
            product.AddImage("http://img.com/2.jpg", 2);

            product.DeleteImages(new[] { "http://img.com/1.jpg", "http://img.com/2.jpg" });

            product.ImageProducts.Should().BeEmpty();
        }

        [Fact]
        public void ClearImages_ShouldClearAll()
        {
            var product = Product.Create(Guid.NewGuid(), "Tee", 10, 5, null, ProductStatus.Available, 1, 1, new List<string>(), Gender.Male, Guid.NewGuid());
            product.AddImage("http://img.com/1.jpg", 1);
            product.AddImage("http://img.com/2.jpg", 2);

            product.ClearImages();

            product.ImageProducts.Should().BeEmpty();
        }
    }
}
