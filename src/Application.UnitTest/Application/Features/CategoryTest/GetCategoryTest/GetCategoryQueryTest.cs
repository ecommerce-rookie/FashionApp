using Application.Features.CategoryFeatures.Models;
using Application.Features.CategoryFeatures.Queries;
using AutoMapper;
using Domain.Aggregates.CategoryAggregate.Entities;
using Domain.Models.Common;
using Domain.Repositories.BaseRepositories;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace Application.UnitTest.Application.Features.CategoryTest.GetCategoryTest
{
    public class GetCategoryQueryHandlerTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly GetCategoryQueryHandler _handler;

        public GetCategoryQueryHandlerTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _handler = new GetCategoryQueryHandler(_mockUnitOfWork.Object, _mockMapper.Object);
        }

        private List<Category> GetFakeCategories()
        {
            return new List<Category>
            {
                new Category(1, "Math"),
                new Category(2, "Science")
            };
        }

        private List<CategoryResponseModel> GetFakeCategoryResponseModels()
        {
            return new List<CategoryResponseModel>
            {
                new CategoryResponseModel { Name = "Math" },
                new CategoryResponseModel { Name = "Science" }
            };
        }

        [Fact]
        public async Task Handle_Should_Return_All_When_PageIsMinusOne_And_SearchIsNull()
        {
            // Arrange
            var query = new GetcategoryQuery { Page = -1, Search = null };
            var fakeCategories = new List<Category> { new("Math") };
            var mappedResult = new PagedList<CategoryResponseModel>(GetFakeCategoryResponseModels());

            _mockUnitOfWork.Setup(u => u.CategoryRepository.GetAll(It.IsAny<Expression<Func<Category, bool>>>()))
                .ReturnsAsync(fakeCategories);

            _mockMapper.Setup(m => m.Map<IEnumerable<CategoryResponseModel>>(fakeCategories)).Returns(mappedResult.Items);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(2);
            result.Items.First().Name.Should().Be("Math");
        }

        [Fact]
        public async Task Handle_Should_Return_All_When_PageIsMinusOne_And_SearchIsProvided()
        {
            // Arrange
            var query = new GetcategoryQuery { Page = -1, Search = "Math" };
            var fakeCategories = new List<Category> { new("Math") };
            var mappedResult = new PagedList<CategoryResponseModel>(new List<CategoryResponseModel> { new() { Name = "Math" } });

            _mockUnitOfWork.Setup(u => u.CategoryRepository.GetAll(It.IsAny<Expression<Func<Category, bool>>>()))
                .ReturnsAsync(fakeCategories);

            _mockMapper.Setup(m => m.Map<IEnumerable<CategoryResponseModel>>(fakeCategories)).Returns(mappedResult.Items);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().ContainSingle(x => x.Name == "Math");
        }

        [Fact]
        public async Task Handle_Should_Return_Paged_When_PageIsPositive_And_SearchIsNull()
        {
            // Arrange
            var query = new GetcategoryQuery { Page = 1, EachPage = 10, Search = null };
            var pagedCategories = new PagedList<Category>(GetFakeCategories());
            var mappedResult = new PagedList<CategoryResponseModel>(new List<CategoryResponseModel> { new() { Name = "Science" } });

            _mockUnitOfWork.Setup(u => u.CategoryRepository.GetAll(
                    It.IsAny<Expression<Func<Category, bool>>>(),
                    query.Page,
                    query.EachPage))
                .ReturnsAsync(pagedCategories);

            _mockMapper.Setup(m => m.Map<PagedList<CategoryResponseModel>>(pagedCategories))
                .Returns(mappedResult);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().ContainSingle(x => x.Name == "Science");
        }

        [Fact]
        public async Task Handle_Should_Return_Paged_When_PageIsPositive_And_SearchIsProvided()
        {
            // Arrange
            var query = new GetcategoryQuery { Page = 1, EachPage = 10, Search = "Art" };
            var pagedCategories = new PagedList<Category>(GetFakeCategories());
            var mappedResult = new PagedList<CategoryResponseModel>(new List<CategoryResponseModel> { new() { Name = "Art" } });

            _mockUnitOfWork.Setup(u => u.CategoryRepository.GetAll(
                    It.IsAny<Expression<Func<Category, bool>>>(),
                    query.Page,
                    query.EachPage))
                .ReturnsAsync(pagedCategories);

            _mockMapper.Setup(m => m.Map<PagedList<CategoryResponseModel>>(pagedCategories))
                .Returns(mappedResult);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().ContainSingle(x => x.Name == "Art");
        }

    }

}
