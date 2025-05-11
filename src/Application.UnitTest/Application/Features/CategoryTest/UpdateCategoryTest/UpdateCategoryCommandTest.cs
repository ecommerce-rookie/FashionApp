using Application.Features.CategoryFeatures.Commands;
using Application.Messages;
using Domain.Aggregates.CategoryAggregate.Entities;
using Domain.Aggregates.CategoryAggregate.Events;
using Domain.Exceptions;
using Domain.Repositories.BaseRepositories;
using FluentAssertions;
using MediatR;
using Moq;
using System.Net;

namespace Application.UnitTest.Application.Features.CategoryTest.UpdateCategoryTest
{
    public class UpdateCategoryCommandHandlerTests
    {
        private readonly UpdateCategoryCommandHandler _handler;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IPublisher> _mockPublisher;

        public UpdateCategoryCommandHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockPublisher = new Mock<IPublisher>();

            _handler = new UpdateCategoryCommandHandler(_mockUnitOfWork.Object, _mockPublisher.Object);
        }

        [Fact]
        public async Task Should_Throw_NotFoundException_When_Category_Does_Not_Exist()
        {
            // Arrange
            var command = new UpdateCategoryCommand { Id = 1, Name = "New Name" };

            _mockUnitOfWork.Setup(x => x.CategoryRepository.GetById(command.Id))
                .ReturnsAsync((Category)null!);

            // Act
            var act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("Category with ID 1 not found.");
        }

        [Fact]
        public async Task Should_Return_InternalServerError_When_Save_Fails()
        {
            // Arrange
            var existingCategory = new Category("Old Name") { Id = 1 };
            var command = new UpdateCategoryCommand { Id = 1, Name = "Updated Name" };

            _mockUnitOfWork.Setup(x => x.CategoryRepository.GetById(command.Id)).ReturnsAsync(existingCategory);
            _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Status.Should().Be(HttpStatusCode.InternalServerError);
            result.Message.Should().Be(MessageCommon.UpdateFailed);
        }

        [Fact]
        public async Task Should_Update_Category_And_Return_Success()
        {
            // Arrange
            var existingCategory = new Category("Old Name") { Id = 1 };
            var command = new UpdateCategoryCommand { Id = 1, Name = "New Name" };

            _mockUnitOfWork.Setup(x => x.CategoryRepository.GetById(command.Id)).ReturnsAsync(existingCategory);
            _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Status.Should().Be(HttpStatusCode.Created);
            result.Message.Should().Be(MessageCommon.UpdateSuccesfully);

            _mockPublisher.Verify(x => x.Publish(It.Is<ModifiedCategoryEvent>(
                e => e.CategoryId == existingCategory.Id && e.CategoryName == command.Name), It.IsAny<CancellationToken>()), Times.Once);

            existingCategory.Name.Should().Be("New Name");
        }
    }
}
