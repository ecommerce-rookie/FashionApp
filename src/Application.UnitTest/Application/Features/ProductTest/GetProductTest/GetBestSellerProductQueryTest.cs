using Application.Features.ProductFeatures.Models;
using Application.Features.ProductFeatures.Queries;
using AutoMapper;
using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Aggregates.ProductAggregate.Enums;
using Domain.Repositories.BaseRepositories;
using FluentAssertions;
using Moq;

namespace Application.UnitTest.Application.Features.ProductTest.GetProductTest
{
    public class GetBestSellerProductQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly GetBestSellerProductQueryHandler _handler;

        public GetBestSellerProductQueryHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _handler = new GetBestSellerProductQueryHandler(_mockUnitOfWork.Object, _mockMapper.Object);
        }

        private static List<Product> GenerateProducts(int count)
        {
            var products = new List<Product>();
            for (int i = 0; i < count; i++)
            {
                products.Add(Product.Create(
                    Guid.NewGuid(),
                    $"Product {i}",
                    100 + i,
                    50 + i,
                    $"Description {i}",
                    ProductStatus.Available,
                    1,
                    10,
                    new List<string> { "S", "M" },
                    Gender.Male,
                    Guid.NewGuid()
                ));
            }

            return products;
        }

        [Fact]
        public async Task Handle_ShouldReturnMappedProducts_WhenProductsExist()
        {
            // Arrange
            var products = GenerateProducts(5);
            var responseModels = products.Select(p => new ProductPreviewResponseModel
            {
                Id = p.Id,
                Name = p.Name
            });

            _mockUnitOfWork.Setup(x => x.ProductRepository.GetBestSeller(5))
                .ReturnsAsync(products);

            _mockMapper.Setup(m => m.Map<IEnumerable<ProductPreviewResponseModel>>(products))
                .Returns(responseModels);

            var query = new GetBestSellerProductQuery { EachPage = 5 };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(5);
            result.Should().BeEquivalentTo(responseModels);
            _mockUnitOfWork.Verify(x => x.ProductRepository.GetBestSeller(5), Times.Once);
            _mockMapper.Verify(x => x.Map<IEnumerable<ProductPreviewResponseModel>>(products), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoProducts()
        {
            // Arrange
            var emptyProducts = new List<Product>();
            var emptyResponse = new List<ProductPreviewResponseModel>();

            _mockUnitOfWork.Setup(x => x.ProductRepository.GetBestSeller(3))
                .ReturnsAsync(emptyProducts);

            _mockMapper.Setup(m => m.Map<IEnumerable<ProductPreviewResponseModel>>(emptyProducts))
                .Returns(emptyResponse);

            var query = new GetBestSellerProductQuery { EachPage = 3 };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            _mockUnitOfWork.Verify(x => x.ProductRepository.GetBestSeller(3), Times.Once);
            _mockMapper.Verify(x => x.Map<IEnumerable<ProductPreviewResponseModel>>(emptyProducts), Times.Once);
        }
    }


}
