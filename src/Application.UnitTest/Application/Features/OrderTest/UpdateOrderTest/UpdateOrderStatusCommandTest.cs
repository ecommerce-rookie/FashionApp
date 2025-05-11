using Application.Features.OrderFeatures.Commands;
using Application.Messages;
using Domain.Aggregates.OrderAggregate.Entities;
using Domain.Aggregates.OrderAggregate.Enums;
using Domain.Repositories.BaseRepositories;
using FluentAssertions;
using Moq;
using System.Net;

namespace Application.UnitTest.Application.Features.OrderTest.UpdateOrderTest
{
    public class UpdateOrderStatusCommandHandlerTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly UpdateOrderStatusCommandHandler _handler;

        public UpdateOrderStatusCommandHandlerTest()
        {
            // Setup mock dependencies
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _handler = new UpdateOrderStatusCommandHandler(_mockUnitOfWork.Object);
        }

        // Test case 1: Order not found
        [Fact]
        public async Task Handle_Should_Return_NotFound_When_Order_Is_Null()
        {
            // Arrange
            var command = new UpdateOrderStatusCommand
            {
                OrderId = Guid.NewGuid(),
                NewStatus = OrderStatus.Shipping
            };

            _mockUnitOfWork.Setup(x => x.OrderRepository.GetById(command.OrderId))
                .ReturnsAsync((Order)null!);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Status.Should().Be(HttpStatusCode.NotFound);
            result.Message.Should().Be(MessageCommon.NotFound);
        }

        // Test case 2: Successfully update order status
        [Fact]
        public async Task Handle_Should_Update_Order_Status_And_Return_Success_When_Save_Succeeds()
        {
            // Arrange
            var order = new Order(12, "Ho Chi Minh", OrderStatus.Pending, PaymentMethod.Cash, "Test", Guid.NewGuid());

            var command = new UpdateOrderStatusCommand
            {
                OrderId = order.Id,
                NewStatus = OrderStatus.Shipping
            };

            _mockUnitOfWork.Setup(x => x.OrderRepository.GetById(command.OrderId))
                .ReturnsAsync(order);

            _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Status.Should().Be(HttpStatusCode.OK);
            result.Message.Should().Be(MessageCommon.UpdateSuccesfully);
            order.OrderStatus.Should().Be(command.NewStatus);
        }

        // Test case 3: Save changes fail
        [Fact]
        public async Task Handle_Should_Return_InternalServerError_When_Save_Fails()
        {
            // Arrange
            var order = new Order(12, "Ho Chi Minh", OrderStatus.Pending, PaymentMethod.Cash, "Test", Guid.NewGuid());
            var command = new UpdateOrderStatusCommand
            {
                OrderId = order.Id,
                NewStatus = OrderStatus.Shipping
            };

            _mockUnitOfWork.Setup(x => x.OrderRepository.GetById(command.OrderId))
                .ReturnsAsync(order);

            _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Status.Should().Be(HttpStatusCode.InternalServerError);
            result.Message.Should().Be(MessageCommon.UpdateFailed);
        }
    }
}
