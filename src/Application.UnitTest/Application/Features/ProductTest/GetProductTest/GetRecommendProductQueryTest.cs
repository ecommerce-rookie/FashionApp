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
    public class GetRecommendProductQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly GetRecommendProductQueryHandler _handler;

        public GetRecommendProductQueryHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _handler = new GetRecommendProductQueryHandler(_mockUnitOfWork.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnMappedProducts_WhenRepositoryReturnsProducts()
        {
            // Arrange
            var slug = "product-slug";
            var eachPage = 4;
            var request = new GetRecommendProductQuery { Slug = slug, EachPage = eachPage };

            var domainProducts = new List<Product>
            {
                Product.Create(
                    id: Guid.NewGuid(),
                    name: "Product 1",
                    unitPrice: 300,
                    purchasePrice: 250,
                    description: "Description",
                    status: ProductStatus.Available,
                    categoryId: 1,
                    quantity: 10,
                    sizes: new List<string> { "M", "XL" },
                    gender: Gender.Female,
                    createdBy: Guid.NewGuid()
                )
            };

            var mappedProducts = domainProducts.Select(p => new ProductPreviewResponseModel
            {
                Id = p.Id,
                Name = p.Name,
                UnitPrice = p.Price.UnitPrice.Amount,
                PurchasePrice = p.Price.PurchasePrice.Amount
            }).ToList();

            _mockUnitOfWork.Setup(x => x.ProductRepository.GetRecommendProduct(slug, eachPage))
                .ReturnsAsync(domainProducts);

            _mockMapper.Setup(x => x.Map<IEnumerable<ProductPreviewResponseModel>>(domainProducts))
                .Returns(mappedProducts);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(mappedProducts);
            _mockUnitOfWork.Verify(x => x.ProductRepository.GetRecommendProduct(slug, eachPage), Times.Once);
            _mockMapper.Verify(x => x.Map<IEnumerable<ProductPreviewResponseModel>>(domainProducts), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenRepositoryReturnsEmptyList()
        {
            // Arrange
            var slug = "product-slug";
            var eachPage = 5;
            var request = new GetRecommendProductQuery { Slug = slug, EachPage = eachPage };

            var domainProducts = new List<Product>();

            _mockUnitOfWork.Setup(x => x.ProductRepository.GetRecommendProduct(slug, eachPage))
                .ReturnsAsync(domainProducts);

            _mockMapper.Setup(x => x.Map<IEnumerable<ProductPreviewResponseModel>>(domainProducts))
                .Returns(new List<ProductPreviewResponseModel>());

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
            _mockUnitOfWork.Verify(x => x.ProductRepository.GetRecommendProduct(slug, eachPage), Times.Once);
            _mockMapper.Verify(x => x.Map<IEnumerable<ProductPreviewResponseModel>>(domainProducts), Times.Once);
        }
    }
}
