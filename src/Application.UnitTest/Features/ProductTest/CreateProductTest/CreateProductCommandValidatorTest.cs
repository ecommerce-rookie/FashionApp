using Application.Features.ProductFeatures.Commands;
using Domain.Aggregates.ProductAggregate.Enums;
using Domain.Repositories;
using Domain.Repositories.BaseRepositories;
using FluentAssertions;
using Moq;

namespace Application.UnitTest.Features.ProductTest.CreateProductTest
{
    public class CreateProductCommandValidatorTests
    {
        private readonly CreateProductCommandValidator _validator;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IProductRepository> _mockProductRepository;

        public CreateProductCommandValidatorTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockProductRepository = new Mock<IProductRepository>();

            // Set up the mock to return a mock ProductRepository
            _mockUnitOfWork.Setup(u => u.ProductRepository).Returns(_mockProductRepository.Object);
            _mockUnitOfWork.Setup(u => u.CategoryRepository).Returns(new Mock<ICategoryRepository>().Object);

            _validator = new CreateProductCommandValidator(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Should_Fail_When_Name_Is_Empty()
        {
            // Arrange
            _mockUnitOfWork.Setup(x => x.CategoryRepository.CheckCategoryExist(It.IsAny<int>())).ReturnsAsync(false);
            var command = new CreateProductCommand { Name = string.Empty };

            // Act
            var result = await _validator.ValidateAsync(command);

            // Assert
            result.Errors.Should().ContainSingle(e => e.PropertyName.Equals("Name") && e.ErrorMessage.Equals("Product name is required"));
        }

        [Fact]
        public async Task Should_Fail_When_UnitPrice_Is_Less_Than_Zero()
        {
            // Arrange
            var command = new CreateProductCommand { UnitPrice = -10 };

            // Act
            var result = await _validator.ValidateAsync(command, CancellationToken.None);

            // Assert
            result.Errors.Should().ContainSingle(e =>
                e.PropertyName == nameof(CreateProductCommand.UnitPrice) &&
                e.ErrorMessage == "Unit price must be assigned with a positive value");
        }

        [Fact]
        public async Task Should_Fail_When_CategoryDoesNotExist()
        {
            // Arrange
            _mockUnitOfWork.Setup(x => x.CategoryRepository.CheckCategoryExist(It.IsAny<int>())).ReturnsAsync(false);
            var command = new CreateProductCommand { CategoryId = 999 }; // Assume category doesn't exist

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
            _mockProductRepository.Setup(x => x.CheckDuplicatedName(It.IsAny<string>())).ReturnsAsync(false); // Mock for name duplication check

            var command = new CreateProductCommand
            {
                Name = "Valid Product",
                UnitPrice = 100,
                PurchasePrice = 80,
                CategoryId = 1,
                Quantity = 10,
                Status = ProductStatus.Available,
                Gender = Gender.Male
            };

            // Act
            var result = await _validator.ValidateAsync(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }
    }



}
