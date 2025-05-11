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

namespace Application.UnitTest.Application.Features.CategoryTest.DeleteCategoryTest
{
    public class DeleteCategoryCommandHandlerTests
    {
        private readonly DeleteCategoryCommandHandler _handler;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IPublisher> _mockPublisher;

        public DeleteCategoryCommandHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockPublisher = new Mock<IPublisher>();
            _handler = new DeleteCategoryCommandHandler(_mockUnitOfWork.Object, _mockPublisher.Object);
        }

        [Fact]
        public async Task Should_Throw_NotFoundException_When_Category_Does_Not_Exist()
        {
            // Arrange
            var command = new DeleteCategoryCommand { Id = 1 };

            _mockUnitOfWork.Setup(x => x.CategoryRepository.GetById(command.Id)).ReturnsAsync((Category)null!);

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
            var category = new Category("Test Category") { Id = 1 };
            var command = new DeleteCategoryCommand { Id = 1 };

            _mockUnitOfWork.Setup(x => x.CategoryRepository.GetById(command.Id)).ReturnsAsync(category);
            _mockUnitOfWork.Setup(x => x.CategoryRepository.Delete(category)).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Status.Should().Be(HttpStatusCode.InternalServerError);
            result.Message.Should().Be(MessageCommon.DeleteFailed);
        }

        [Fact]
        public async Task Should_Delete_Category_And_Publish_Event_When_Successful()
        {
            // Arrange
            var category = new Category("To Be Deleted") { Id = 10 };
            var command = new DeleteCategoryCommand { Id = 10 };

            _mockUnitOfWork.Setup(x => x.CategoryRepository.GetById(command.Id)).ReturnsAsync(category);
            _mockUnitOfWork.Setup(x => x.CategoryRepository.Delete(category)).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Status.Should().Be(HttpStatusCode.Created);
            result.Message.Should().Be(MessageCommon.DeleteSuccessfully);

            _mockPublisher.Verify(x => x.Publish(It.Is<ModifiedCategoryEvent>(
                e => e.CategoryId == category.Id && e.CategoryName == category.Name),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
