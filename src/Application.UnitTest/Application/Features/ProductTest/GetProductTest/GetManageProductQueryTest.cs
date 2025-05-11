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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Application.UnitTest.Application.Features.ProductTest.GetProductTest
{
    public class GetManageProductQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly GetManageProductQueryHandler _handler;

        public GetManageProductQueryHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _handler = new GetManageProductQueryHandler(_mockUnitOfWork.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnProduct_WhenProductExists()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var slug = $"test-slug-{productId}";
            var product = Product.Create(
                id: productId,
                name: "Test Product",
                unitPrice: 100,
                purchasePrice: 80,
                description: "Test Description",
                status: ProductStatus.Available,
                categoryId: 1,
                quantity: 10,
                sizes: new List<string> { "S", "M" },
                gender: Gender.Unisex,
                createdBy: Guid.NewGuid()
            );

            var productResponse = new ProductResponseModel
            {
                Id = product.Id,
                Name = product.Name,
                // Populate other needed properties...
            };

            _mockUnitOfWork.Setup(x => x.ProductRepository.GetManageDetail(slug))
                .ReturnsAsync(product);

            _mockMapper.Setup(x => x.Map<ProductResponseModel>(product))
                .Returns(productResponse);

            var request = new GetManageProductQuery { Slug = slug };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(HttpStatusCode.OK);
            result.Message.Should().Be(MessageCommon.GetSuccesfully);
            result.Data.Should().BeEquivalentTo(productResponse);
        }


        [Fact]
        public async Task Handle_ShouldThrowValidationException_WhenProductNotFound()
        {
            // Arrange
            var slug = "non-existent-slug";
            _mockUnitOfWork.Setup(x => x.ProductRepository.GetManageDetail(slug))
                .ReturnsAsync((Product?)null);

            var request = new GetManageProductQuery { Slug = slug };

            // Act
            Func<Task> act = async () => await _handler.Handle(request, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<ValidationException>()
                .WithMessage("One or more validation failures have occurred.")
                .Where(ex => ex.Errors.Any(e => e.Key == nameof(request.Slug)));
        }

    }

}
