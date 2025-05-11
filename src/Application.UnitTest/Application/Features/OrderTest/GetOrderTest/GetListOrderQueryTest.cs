using Algolia.Search.Models.Search;
using Application.Features.OrderFeatures.Models;
using Application.Features.OrderFeatures.Queries;
using AutoMapper;
using Domain.Aggregates.OrderAggregate.Entities;
using Domain.Aggregates.OrderAggregate.Enums;
using Domain.Aggregates.OrderAggregate.ValuesObject;
using Domain.Aggregates.ProductAggregate.ValuesObjects;
using Domain.Aggregates.UserAggregate.Enums;
using Domain.Models.Common;
using Domain.Repositories.BaseRepositories;
using FluentAssertions;
using Infrastructure.Authentication.Services;
using Infrastructure.Authentication.Settings;
using Moq;
using System.Linq.Expressions;

namespace Application.UnitTest.Application.Features.OrderTest.GetOrderTest
{
    public class GetListOrderQueryHandlerTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IAuthenticationService> _mockAuthService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly GetListOrderQueryHandler _handler;

        public GetListOrderQueryHandlerTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockAuthService = new Mock<IAuthenticationService>();
            _mockMapper = new Mock<IMapper>();
            _handler = new GetListOrderQueryHandler(_mockUnitOfWork.Object, _mockAuthService.Object, _mockMapper.Object);
        }

        public static List<Order> GenerateOrdersWithDetails(int orderCount, int detailsPerOrder = 2, Guid? customerId = null)
        {
            var orders = new List<Order>();
            var rnd = new Random();

            for (int i = 0; i < orderCount; i++)
            {
                var custId = customerId ?? Guid.NewGuid();
                var order = Order.Create(
                    totalPrice: rnd.Next(100, 1000),
                    address: $"Address {i}",
                    status: i % 2 == 0 ? OrderStatus.Pending : OrderStatus.Delivered,
                    method: i % 2 == 0 ? PaymentMethod.Cash : PaymentMethod.VnPay,
                    nameReceiver: $"Receiver {i}",
                    customerId: custId
                );

                var carts = new List<Cart>();
                for (int j = 0; j < detailsPerOrder; j++)
                {
                    carts.Add(new Cart
                    {
                        ProductId = Guid.NewGuid(),
                        Quantity = rnd.Next(1, 5),
                        Price = new Money(rnd.Next(10, 100))
                    });
                }

                order.CreateOrderDetail(carts);
                orders.Add(order);
            }

            return orders;
        }

        [Fact]
        public async Task Handle_Should_Return_Orders_For_Customer()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var orders = GenerateOrdersWithDetails(5, 2);

            var userMock = new UserAuthenModel
            {
                Role = UserRole.Customer,
                UserId = userId
            };
            _mockAuthService.Setup(x => x.User).Returns(userMock);

            _mockUnitOfWork.Setup(u => u.OrderRepository.GetOrderCustomer(userId))
                .ReturnsAsync(orders);

            _mockMapper.Setup(m => m.Map<PagedList<OrderResponseModel>>(It.IsAny<PagedList<Order>>()))
                .Returns(new PagedList<OrderResponseModel>());

            var query = new GetListOrderQuery { Page = 1, EachPage = 10 };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            _mockUnitOfWork.Verify(u => u.OrderRepository.GetOrderCustomer(userId), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Return_Orders_For_Admin_With_No_Filter()
        {
            // Arrange  
            var query = new GetListOrderQuery { Page = 1, EachPage = 10 };
            var orders = new PagedList<Order>(GenerateOrdersWithDetails(5, 2));

            var userMock = new UserAuthenModel
            {
                Role = UserRole.Admin,
                UserId = Guid.NewGuid()
            };
            _mockAuthService.Setup(x => x.User).Returns(userMock);

            _mockUnitOfWork.Setup(u => u.OrderRepository.GetAll(
                    It.IsAny<Expression<Func<Order, bool>>>(),
                    query.Page, query.EachPage,
                    OrderSortBy.CreatedAt.ToString(), true,
                    It.IsAny<Expression<Func<Order, object>>>(),
                    It.IsAny<Expression<Func<Order, object>>>()))
                .ReturnsAsync(orders);

            _mockMapper.Setup(m => m.Map<PagedList<OrderResponseModel>>(orders))
                .Returns(new PagedList<OrderResponseModel>());

            // Act  
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert  
            result.Should().NotBeNull();
            _mockUnitOfWork.Verify(u => u.OrderRepository.GetAll(
                It.IsAny<Expression<Func<Order, bool>>>(),
                query.Page, query.EachPage,
                OrderSortBy.CreatedAt.ToString(), true,
                It.IsAny<Expression<Func<Order, object>>>(),
                It.IsAny<Expression<Func<Order, object>>>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Filter_By_Search_Payment_OrderStatus()
        {
            // Arrange
            var query = new GetListOrderQuery
            {
                Page = 1,
                EachPage = 10,
                Search = "Address",
                PaymentMethods = new[] { PaymentMethod.VnPay },
                OrderStatuss = new[] { OrderStatus.Delivered }
            };
            var orders = new PagedList<Order>(GenerateOrdersWithDetails(5, 2));

            var userMock = new UserAuthenModel
            {
                Role = UserRole.Admin,
                UserId = Guid.NewGuid()
            };
            _mockAuthService.Setup(x => x.User).Returns(userMock);

            _mockUnitOfWork.Setup(u => u.OrderRepository.GetAll(
                    It.IsAny<Expression<Func<Order, bool>>>(),
                    query.Page, query.EachPage,
                    OrderSortBy.CreatedAt.ToString(), true,
                    It.IsAny<Expression<Func<Order, object>>>(),
                    It.IsAny<Expression<Func<Order, object>>>()))
                .ReturnsAsync(orders);

            _mockMapper.Setup(m => m.Map<PagedList<OrderResponseModel>>(orders))
                .Returns(new PagedList<OrderResponseModel>());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task Handle_Should_Do_Nothing_If_Role_Not_Handled()
        {
            // Arrange
            var query = new GetListOrderQuery { Page = 1, EachPage = 10 };

            var userMock = new UserAuthenModel
            {
                Role = UserRole.Customer,
                UserId = Guid.NewGuid()
            };
            _mockAuthService.Setup(x => x.User).Returns(userMock);
            _mockUnitOfWork.Setup(u => u.OrderRepository.GetOrderCustomer(It.IsAny<Guid>()))
                .ReturnsAsync(new List<Order>());
            _mockMapper.Setup(m => m.Map<PagedList<OrderResponseModel>>(It.IsAny<PagedList<Order>>()))
                .Returns(new PagedList<OrderResponseModel>());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<PagedList<OrderResponseModel>>();
        }
    }
}
