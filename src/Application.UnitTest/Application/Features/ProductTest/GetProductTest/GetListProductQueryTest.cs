using Application.Features.ProductFeatures.Enums;
using Application.Features.ProductFeatures.Models;
using Application.Features.ProductFeatures.Queries;
using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Models.Common;
using Domain.Repositories.BaseRepositories;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace Application.UnitTest.Application.Features.ProductTest.GetProductTest
{
    public class GetListProductQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly GetListProductQueryHandler _handler;

        public GetListProductQueryHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _handler = new GetListProductQueryHandler(_mockUnitOfWork.Object);
        }

        private static PagedList<ProductPreviewResponseModel> GenerateProductPreviewPagedList(int total = 5, int page = 1, int eachPage = 5)
        {
            var items = Enumerable.Range(1, total).Select(i =>
                new ProductPreviewResponseModel
                {
                    Id = Guid.NewGuid(),
                    Name = $"Product {i}",
                    UnitPrice = 100 + i,
                }
            ).ToList();

            return new PagedList<ProductPreviewResponseModel>(items, total, page, eachPage);
        }

        [Fact]
        public async Task Handle_ShouldReturnResult_WhenAllFiltersAreNull()
        {
            var query = new GetListProductQuery
            {
                Page = 1,
                EachPage = 10
            };

            _mockUnitOfWork.Setup(x => x.ProductRepository.GetAll(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<Expression<Func<Product, ProductPreviewResponseModel>>>(),
                1, 10,
                It.IsAny<string>(), true))
                .ReturnsAsync(GenerateProductPreviewPagedList());

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Should().NotBeNull();
            result.Count().Should().Be(5);
        }

        [Fact]
        public async Task Handle_ShouldReturnResult_WithAllFilters()
        {
            var query = new GetListProductQuery
            {
                Page = 1,
                EachPage = 10,
                Categories = new List<int> { 1, 2 },
                Sizes = new List<string> { "M", "L" },
                Search = "Test"
            };

            _mockUnitOfWork.Setup(x => x.ProductRepository.GetAll(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<Expression<Func<Product, ProductPreviewResponseModel>>>(),
                1, 10,
                It.IsAny<string>(), true))
                .ReturnsAsync(GenerateProductPreviewPagedList());

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task Handle_ShouldReturnResult_WithPriceFilter()
        {
            var query = new GetListProductQuery
            {
                Page = 1,
                EachPage = 10,
                MinPrice = 50,
                MaxPrice = 200
            };

            _mockUnitOfWork.Setup(x => x.ProductRepository.GetAll(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<Expression<Func<Product, ProductPreviewResponseModel>>>(),
                1, 10,
                It.IsAny<string>(), true))
                .ReturnsAsync(GenerateProductPreviewPagedList());

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task Handle_ShouldReturnResult_WithIsNewTrue()
        {
            var query = new GetListProductQuery
            {
                Page = 1,
                EachPage = 10,
                IsNew = true
            };

            _mockUnitOfWork.Setup(x => x.ProductRepository.GetAll(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<Expression<Func<Product, ProductPreviewResponseModel>>>(),
                1, 10,
                It.IsAny<string>(), true))
                .ReturnsAsync(GenerateProductPreviewPagedList());

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task Handle_ShouldReturnResult_WithIsNewFalseAndIsSaleTrue()
        {
            var query = new GetListProductQuery
            {
                Page = 1,
                EachPage = 10,
                IsNew = false,
                IsSale = true
            };

            _mockUnitOfWork.Setup(x => x.ProductRepository.GetAll(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<Expression<Func<Product, ProductPreviewResponseModel>>>(),
                1, 10,
                It.IsAny<string>(), true))
                .ReturnsAsync(GenerateProductPreviewPagedList());

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task Handle_ShouldReturnResult_WithSortingAndSaleFalse()
        {
            var query = new GetListProductQuery
            {
                Page = 1,
                EachPage = 10,
                IsSale = false,
                SortBy = ProductSortBy.Name,
                IsAscending = false
            };

            _mockUnitOfWork.Setup(x => x.ProductRepository.GetAll(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<Expression<Func<Product, ProductPreviewResponseModel>>>(),
                1, 10,
                "Name", false))
                .ReturnsAsync(GenerateProductPreviewPagedList());

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Should().NotBeNull();
        }


        [Fact]
        public async Task Handle_ShouldReturnEmpty_WhenNoProductFound()
        {
            var query = new GetListProductQuery
            {
                Page = 1,
                EachPage = 10
            };

            _mockUnitOfWork.Setup(x => x.ProductRepository.GetAll(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<Expression<Func<Product, ProductPreviewResponseModel>>>(),
                1, 10,
                It.IsAny<string>(), true))
                .ReturnsAsync(new PagedList<ProductPreviewResponseModel>([], 0, 1, 10));

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Should().NotBeNull();
            result.Count().Should().Be(0);
        }


    }
}
