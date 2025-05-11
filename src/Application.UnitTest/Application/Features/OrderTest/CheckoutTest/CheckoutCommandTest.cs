using Algolia.Search.Models.Insights;
using Application.Features.OrderFeatures.Commands;
using Application.Features.OrderFeatures.Models;
using Application.Messages;
using Domain.Aggregates.OrderAggregate.Entities;
using Domain.Aggregates.OrderAggregate.Enums;
using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Aggregates.ProductAggregate.Enums;
using Domain.Aggregates.ProductAggregate.ValuesObjects;
using Domain.Aggregates.UserAggregate.Enums;
using Domain.Repositories.BaseRepositories;
using FluentAssertions;
using Infrastructure.Authentication.Services;
using Infrastructure.Authentication.Settings;
using Moq;
using System.Linq.Expressions;
using System.Net;

namespace Application.UnitTest.Application.Features.OrderTest.CheckoutTest
{
    public class CheckoutCommandHandlerTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IAuthenticationService> _authMock;
        private readonly CheckoutCommandHandler _handler;
        private readonly Guid _userId;

        public CheckoutCommandHandlerTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _authMock = new Mock<IAuthenticationService>();
            _userId = Guid.NewGuid();

            var userMock = new UserAuthenModel
            {
                Role = UserRole.Customer,
                UserId = _userId
            };

            _authMock.Setup(x => x.User).Returns(userMock);
            _handler = new CheckoutCommandHandler(_unitOfWorkMock.Object, _authMock.Object);
        }

        private List<Product> CreateProductList(int count)
        {
            var products = new List<Product>();
            for (int i = 0; i < count; i++)
            {
                var product = new Product(
                    id: Guid.NewGuid(),
                    name: $"Product {i + 1}",
                    unitPrice: 100m + i,
                    purchasePrice: 50m + i,
                    description: $"Description for Product {i + 1}",
                    status: i % 2 == 0 ? ProductStatus.Available : ProductStatus.OutOfStock,
                    categoryId: 1,
                    quantity: i + 10,
                    sizes: new List<string> { "S", "M", "L" },
                    gender: Gender.Male,
                    createdBy: Guid.NewGuid()
                );
                products.Add(product);
            }
            return products;
        }


        [Fact]
        public async Task Handle_ShouldReturnCreated_WhenOrderProcessedSuccessfully()
        {
            // Arrange
            var products = CreateProductList(5);
            var carts = new List<CartRequestModel>
            {
                new() { ProductId = products.ElementAt(0).Id, Quantity = 2 }
            };


            _unitOfWorkMock.Setup(u => u.ProductRepository.GetAll(It.IsAny<Expression<Func<Product, bool>>>()))
                           .ReturnsAsync(products);
            _unitOfWorkMock.Setup(x => x.OrderRepository.Add(It.IsAny<Order>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.OrderRepository.AddRangeOrderDetail(It.IsAny<IEnumerable<OrderDetail>>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                           .ReturnsAsync(true);

            var command = new CheckoutCommand
            {
                Carts = carts,
                Address = "123 Street",
                NameReceiver = "Alice",
                PaymentMethod = PaymentMethod.Cash
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Status.Should().Be(HttpStatusCode.Created);
            result.Message.Should().Be(MessageCommon.CreateSuccesfully);
        }

        [Fact]
        public async Task Handle_ShouldReturnInternalServerError_WhenSaveFails()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var carts = new List<CartRequestModel>
            {
                new() { ProductId = productId, Quantity = 1 }
            };

            var products = CreateProductList(5);

            _unitOfWorkMock.Setup(u => u.ProductRepository.GetAll(It.IsAny<Expression<Func<Product, bool>>>()))
                           .ReturnsAsync(products);
            _unitOfWorkMock.Setup(x => x.OrderRepository.Add(It.IsAny<Order>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.OrderRepository.AddRangeOrderDetail(It.IsAny<IEnumerable<OrderDetail>>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                           .ReturnsAsync(false);

            var command = new CheckoutCommand
            {
                Carts = carts,
                Address = "456 Avenue",
                NameReceiver = "Bob",
                PaymentMethod = PaymentMethod.Cash
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Status.Should().Be(HttpStatusCode.InternalServerError);
            result.Message.Should().Be(MessageCommon.CreateFailed);
        }

        [Fact]
        public async Task Handle_ShouldUseZeroPrice_WhenProductPriceIsNull()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var carts = new List<CartRequestModel>
            {
                new() { ProductId = productId, Quantity = 3 }
            };

            var products = CreateProductList(5);

            _unitOfWorkMock.Setup(u => u.ProductRepository.GetAll(It.IsAny<Expression<Func<Product, bool>>>()))
                           .ReturnsAsync(products);
            _unitOfWorkMock.Setup(x => x.OrderRepository.Add(It.IsAny<Order>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.OrderRepository.AddRangeOrderDetail(It.IsAny<IEnumerable<OrderDetail>>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                           .ReturnsAsync(true);

            var command = new CheckoutCommand
            {
                Carts = carts,
                Address = "789 Road",
                NameReceiver = "Charlie",
                PaymentMethod = PaymentMethod.Cash
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Status.Should().Be(HttpStatusCode.Created);
            result.Message.Should().Be(MessageCommon.CreateSuccesfully);
        }
    }
}
