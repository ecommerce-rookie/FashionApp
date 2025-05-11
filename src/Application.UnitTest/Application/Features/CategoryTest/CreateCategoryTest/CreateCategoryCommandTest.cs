using Application.Features.CategoryFeatures.Commands;
using Application.Messages;
using Domain.Aggregates.CategoryAggregate.Entities;
using Domain.Aggregates.CategoryAggregate.Events;
using Domain.Repositories.BaseRepositories;
using FluentAssertions;
using FluentValidation.TestHelper;
using MediatR;
using Moq;
using System.Net;

namespace Application.UnitTest.Application.Features.CategoryTest.CreateCategoryTest
{
    public class CreateCategoryCommandHandlerTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IPublisher> _mockPublisher;
        private readonly CreateCategoryCommandHandler _handler;

        public CreateCategoryCommandHandlerTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockPublisher = new Mock<IPublisher>();

            _handler = new CreateCategoryCommandHandler(
                _mockUnitOfWork.Object,
                _mockPublisher.Object
            );
        }

        [Fact]
        public async Task Handle_Should_Return_Created_When_Category_Is_Valid()
        {
            // Arrange
            var command = new CreateCategoryCommand
            {
                Name = "Technology"
            };

            _mockUnitOfWork
                .Setup(x => x.CategoryRepository.Add(It.IsAny<Category>()))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Status.Should().Be(HttpStatusCode.Created);
            result.Message.Should().Be(MessageCommon.CreateSuccesfully);

            _mockUnitOfWork.Verify(x => x.CategoryRepository.Add(It.Is<Category>(c => c.Name == "Technology")), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockPublisher.Verify(x => x.Publish(It.IsAny<ModifiedCategoryEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Return_InternalServerError_When_Save_Fails()
        {
            // Arrange
            var command = new CreateCategoryCommand
            {
                Name = "Fashion"
            };

            _mockUnitOfWork
                .Setup(x => x.CategoryRepository.Add(It.IsAny<Category>()))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Status.Should().Be(HttpStatusCode.InternalServerError);
            result.Message.Should().Be(MessageCommon.CreateFailed);

            _mockPublisher.Verify(x => x.Publish(It.IsAny<ModifiedCategoryEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }

    public class CreateCategoryCommandValidatorTests
    {
        private readonly CreateCategoryCommandValidator _validator = new();

        [Fact]
        public void Validate_Should_Have_Error_When_Name_Is_Empty()
        {
            var command = new CreateCategoryCommand { Name = string.Empty };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Validate_Should_Have_Error_When_Name_Exceeds_Max_Length()
        {
            var command = new CreateCategoryCommand { Name = new string('x', 101) };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Validate_Should_Not_Have_Error_When_Name_Is_Valid()
        {
            var command = new CreateCategoryCommand { Name = "Books" };
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveValidationErrorFor(x => x.Name);
        }
    }
}
