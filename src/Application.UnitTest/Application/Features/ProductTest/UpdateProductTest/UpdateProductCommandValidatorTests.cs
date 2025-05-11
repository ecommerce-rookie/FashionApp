using Application.Features.ProductFeatures.Commands;
using Domain.Aggregates.ProductAggregate.Enums;
using Domain.Repositories;
using Domain.Repositories.BaseRepositories;
using FluentAssertions;
using Moq;

namespace Application.UnitTest.Application.Features.ProductTest.UpdateProductTest
{
    public class UpdateProductCommandValidatorTests
    {
        private readonly UpdateProductCommandValidator _validator;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IProductRepository> _mockProductRepository;

        public UpdateProductCommandValidatorTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockProductRepository = new Mock<IProductRepository>();

            // Set up the mock to return a mock ProductRepository
            _mockUnitOfWork.Setup(u => u.ProductRepository).Returns(_mockProductRepository.Object);
            _mockUnitOfWork.Setup(u => u.CategoryRepository).Returns(new Mock<ICategoryRepository>().Object);

            _validator = new UpdateProductCommandValidator(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Should_Fail_When_UnitPrice_Is_Less_Than_Zero()
        {
            // Arrange
            _mockUnitOfWork.Setup(x => x.CategoryRepository.CheckCategoryExist(It.IsAny<int>())).ReturnsAsync(false);
            var command = new UpdateProductCommand { UnitPrice = -1 };

            // Act
            var result = await _validator.ValidateAsync(command);

            // Assert
            result.Errors.Should().ContainSingle(e => e.PropertyName.Equals("UnitPrice") && e.ErrorMessage.Equals("Unit price must be assigned with a positive value"));
        }

        [Fact]
        public async Task Should_Fail_When_CategoryDoesNotExist()
        {
            // Arrange
            _mockUnitOfWork.Setup(x => x.CategoryRepository.CheckCategoryExist(It.IsAny<int>())).ReturnsAsync(false);
            var command = new UpdateProductCommand { CategoryId = 999 }; // Assume category doesn't exist

            // Act
            var result = await _validator.ValidateAsync(command);

            // Assert
            result.Errors.Should().ContainSingle(e => e.PropertyName.Equals("CategoryId") && e.ErrorMessage.Equals("This category does not exist"));
        }

        [Fact]
        public async Task Should_Succeed_When_Data_Is_Valid()
        {
            // Arrange
            _mockUnitOfWork.Setup(x => x.CategoryRepository.CheckCategoryExist(It.IsAny<int>())).ReturnsAsync(true);
            var command = new UpdateProductCommand
            {
                Name = "Updated Product",
                UnitPrice = 100,
                PurchasePrice = 80,
                CategoryId = 1,
                Quantity = 10,
                Status = ProductStatus.OutOfStock,
                Gender = Gender.Female
            };

            // Act
            var result = await _validator.ValidateAsync(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }
    }

}
