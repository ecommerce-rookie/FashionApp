using Application.Features.ProductFeatures.Models;
using Application.Features.ProductFeatures.Queries;
using AutoMapper;
using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Aggregates.ProductAggregate.Enums;
using Domain.Models.Common;
using Domain.Repositories.BaseRepositories;
using FluentAssertions;
using Moq;

namespace Application.UnitTest.Application.Features.ProductTest.GetProductTest
{
    public class GetListManageProductQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly GetListManageProductQueryHandler _handler;

        public GetListManageProductQueryHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _handler = new GetListManageProductQueryHandler(_mockUnitOfWork.Object, _mockMapper.Object);
        }

        private static PagedList<Product> GenerateProductPagedList(int total = 5, int page = 1, int eachPage = 5)
        {
            var items = Enumerable.Range(1, total).Select(i =>
                Product.Create(Guid.NewGuid(), $"Product {i}", 100 + i, 50 + i, $"Description {i}",
                    ProductStatus.Available, 1, 10, new List<string> { "M" }, Gender.Male, Guid.NewGuid())
            ).ToList();

            return new PagedList<Product>(items, total, page, eachPage);
        }

        private static PagedList<ProductPreviewManageResponesModel> GenerateProductPreviewPagedList(int total = 5, int page = 1, int eachPage = 5)
        {
            var items = Enumerable.Range(1, total).Select(i =>
                new ProductPreviewManageResponesModel
                {
                    Id = Guid.NewGuid(),
                    Name = $"Product {i}",
                    UnitPrice = 100 + i,
                }
            ).ToList();

            return new PagedList<ProductPreviewManageResponesModel>(items, total, page, eachPage);
        }

        [Fact]
        public async Task Handle_ShouldReturnMappedPagedList_WhenProductsExist()
        {
            // Arrange
            var query = new GetListManageProductQuery
            {
                Page = 1,
                EachPage = 5,
                IsDeleted = false,
                Categories = new[] { 1, 2 },
                Search = "Product",
                Sizes = new[] { "M", "L" }
            };

            var source = GenerateProductPagedList();
            var mapped = GenerateProductPreviewPagedList();

            _mockUnitOfWork.Setup(x =>
                    x.ProductRepository.GetManageProducts(query.Page, query.EachPage, query.IsDeleted,
                        query.Search, query.Categories, query.Sizes))
                .ReturnsAsync(source);

            _mockMapper.Setup(m =>
                    m.Map<PagedList<ProductPreviewManageResponesModel>>(source))
                .Returns(mapped);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(5);
            result.Should().BeEquivalentTo(mapped);
            _mockUnitOfWork.Verify(x =>
                x.ProductRepository.GetManageProducts(query.Page, query.EachPage, query.IsDeleted,
                    query.Search, query.Categories, query.Sizes), Times.Once);
            _mockMapper.Verify(x => x.Map<PagedList<ProductPreviewManageResponesModel>>(source), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyPagedList_WhenNoProducts()
        {
            // Arrange
            var query = new GetListManageProductQuery
            {
                Page = 1,
                EachPage = 10,
                IsDeleted = true,
                Categories = null,
                Search = null,
                Sizes = null
            };

            var emptySource = new PagedList<Product>(new List<Product>(), 0, 1, 10);
            var emptyMapped = new PagedList<ProductPreviewManageResponesModel>(new List<ProductPreviewManageResponesModel>(), 0, 1, 10);

            _mockUnitOfWork.Setup(x =>
                    x.ProductRepository.GetManageProducts(query.Page, query.EachPage, query.IsDeleted,
                        query.Search, query.Categories, query.Sizes))
                .ReturnsAsync(emptySource);

            _mockMapper.Setup(x => x.Map<PagedList<ProductPreviewManageResponesModel>>(emptySource))
                .Returns(emptyMapped);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            result.Count().Should().Be(0);
            _mockUnitOfWork.Verify(x =>
                x.ProductRepository.GetManageProducts(query.Page, query.EachPage, query.IsDeleted,
                    query.Search, query.Categories, query.Sizes), Times.Once);
            _mockMapper.Verify(x => x.Map<PagedList<ProductPreviewManageResponesModel>>(emptySource), Times.Once);
        }
    }

}
