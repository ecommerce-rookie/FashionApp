using Application.Features.ProductFeatures.Models;
using Application.Features.ProductFeatures.Queries;
using AutoMapper;
using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Aggregates.ProductAggregate.Enums;
using Domain.Repositories.BaseRepositories;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace Application.UnitTest.Application.Features.ProductTest.GetProductTest
{
    public class GetProductIdsQueryHandlerTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly GetProductIdsQueryHandler _handler;

        public GetProductIdsQueryHandlerTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _handler = new GetProductIdsQueryHandler(_mockUnitOfWork.Object, _mockMapper.Object);
        }
        private List<Product> CreateFakeProducts(List<Guid> ids)
        {
            var result = new List<Product>();
            int index = 1;
            foreach (var id in ids)
            {
                var product = Product.Create(
                    id: id,
                    name: $"Product {index}",
                    unitPrice: 100 + index,
                    purchasePrice: 90 + index,
                    description: $"Description {index}",
                    status: ProductStatus.Available,
                    categoryId: index,
                    quantity: 10,
                    sizes: new List<string> { "M", "L" },
                    gender: Gender.Male,
                    createdBy: Guid.NewGuid()
                );
                result.Add(product);
                index++;
            }
            return result;
        }


        [Fact]
        public async Task Handle_ShouldReturnMappedProducts_WhenProductsExist()
        {
            // Arrange
            var productId1 = Guid.NewGuid();
            var productId2 = Guid.NewGuid();
            var request = new GetProductIdsQuery
            {
                ProductIds = new List<Guid> { productId1, productId2 }
            };

            var domainProducts = CreateFakeProducts(new List<Guid> { productId1, productId2 });

            var expected = new List<ProductPreviewResponseModel>
            {
                new() { Id = productId1, Name = "Product 1" },
                new() { Id = productId2, Name = "Product 2" }
            };

            _mockUnitOfWork.Setup(x => x.ProductRepository.GetAll(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<Expression<Func<Product, object>>>(),
                It.IsAny<Expression<Func<Product, object>>>()))
                .ReturnsAsync(domainProducts);

            _mockMapper.Setup(x => x.Map<IEnumerable<ProductPreviewResponseModel>>(domainProducts))
                .Returns(expected);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expected);
            _mockUnitOfWork.Verify(x => x.ProductRepository.GetAll(It.IsAny<Expression<Func<Product, bool>>>(),
                                                                   It.IsAny<Expression<Func<Product, object>>>(),
                                                                   It.IsAny<Expression<Func<Product, object>>>()), Times.Once);
            _mockMapper.Verify(x => x.Map<IEnumerable<ProductPreviewResponseModel>>(domainProducts), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoProductsFound()
        {
            // Arrange
            var request = new GetProductIdsQuery
            {
                ProductIds = new List<Guid> { Guid.NewGuid() }
            };

            var emptyList = new List<Product>();

            _mockUnitOfWork.Setup(x => x.ProductRepository.GetAll(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<Expression<Func<Product, object>>>(),
                It.IsAny<Expression<Func<Product, object>>>()))
                .ReturnsAsync(emptyList);

            _mockMapper.Setup(x => x.Map<IEnumerable<ProductPreviewResponseModel>>(emptyList))
                .Returns(new List<ProductPreviewResponseModel>());

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenProductIdsIsEmpty()
        {
            // Arrange
            var request = new GetProductIdsQuery
            {
                ProductIds = new List<Guid>()
            };

            _mockUnitOfWork.Setup(x => x.ProductRepository.GetAll(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<Expression<Func<Product, object>>>(),
                It.IsAny<Expression<Func<Product, object>>>()))
                .ReturnsAsync(new List<Product>());

            _mockMapper.Setup(x => x.Map<IEnumerable<ProductPreviewResponseModel>>(It.IsAny<IEnumerable<Product>>()))
                .Returns(new List<ProductPreviewResponseModel>());

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }


    }
}
