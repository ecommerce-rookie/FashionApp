using Application.Features.ProductFeatures.Models;
using Application.Features.ProductFeatures.Queries;
using Application.Messages;
using AutoMapper;
using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Aggregates.ProductAggregate.Enums;
using Domain.Exceptions;
using Domain.Repositories.BaseRepositories;
using FluentAssertions;
using Moq;
using System.Net;

namespace Application.UnitTest.Application.Features.ProductTest.GetProductTest
{
    public class GetProductQueryHandlerTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly GetProductQueryHandler _handler;

        public GetProductQueryHandlerTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _handler = new GetProductQueryHandler(_mockUnitOfWork.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnMappedProduct_WhenProductExists()
        {
            // Arrange
            var slug = "sample-product";
            var request = new GetProductQuery { Slug = slug };

            var domainProduct = Product.Create(
                id: Guid.NewGuid(),
                name: "Test Product",
                unitPrice: 200,
                purchasePrice: 150,
                description: "Description",
                status: ProductStatus.Available,
                categoryId: 1,
                quantity: 20,
                sizes: new List<string> { "M", "L" },
                gender: Gender.Male,
                createdBy: Guid.NewGuid()
            );

            var mappedProduct = new ProductResponseModel
            {
                Id = domainProduct.Id,
                Name = domainProduct.Name
            };

            _mockUnitOfWork.Setup(x => x.ProductRepository.GetDetail(slug))
                .ReturnsAsync(domainProduct);

            _mockMapper.Setup(x => x.Map<ProductResponseModel>(domainProduct))
                .Returns(mappedProduct);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(HttpStatusCode.OK);
            result.Data.Should().BeEquivalentTo(mappedProduct);
            result.Message.Should().Be(MessageCommon.GetSuccesfully);

            _mockUnitOfWork.Verify(x => x.ProductRepository.GetDetail(slug), Times.Once);
            _mockMapper.Verify(x => x.Map<ProductResponseModel>(domainProduct), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowValidationException_WhenProductNotFound()
        {
            // Arrange
            var slug = "not-found-slug";
            var request = new GetProductQuery { Slug = slug };

            _mockUnitOfWork.Setup(x => x.ProductRepository.GetDetail(slug))
                .ReturnsAsync((Product?)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("One or more validation failures have occurred.")
                .Where(ex => ex.Errors.Any(e => e.Key == nameof(request.Slug)));

            _mockUnitOfWork.Verify(x => x.ProductRepository.GetDetail(slug), Times.Once);
            _mockMapper.Verify(x => x.Map<ProductResponseModel>(It.IsAny<Product>()), Times.Never);
        }
    }
}
