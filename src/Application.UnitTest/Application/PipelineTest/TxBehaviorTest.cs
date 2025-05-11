using Domain.Repositories.BaseRepositories;
using Domain.SeedWorks.Events;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Persistence.SeedWorks.Abstractions;
using RookieShop.Persistence;

namespace Application.UnitTest.Application.PipelineTest
{
    public class TxBehaviorTest
    {
        private readonly Mock<IPublisher> _publisherMock = new();
        private readonly Mock<IDomainEventContext> _eventContextMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<ILogger<TxBehavior<ITxRequest, string>>> _loggerMock = new();

        private readonly TxBehavior<ITxRequest, string> _txBehavior;

        public TxBehaviorTest()
        {
            _txBehavior = new TxBehavior<ITxRequest, string>(
                _publisherMock.Object,
                _eventContextMock.Object,
                _unitOfWorkMock.Object,
                _loggerMock.Object
            );
        }

        public interface ITxRequest : IRequest<string> { }

        public class NonTxRequest : IRequest<string> { }

        public class SampleDomainEvent : BaseEvent { }

        [Fact]
        public async Task Handle_Should_Invoke_Next_Directly_If_Not_ITxRequest()
        {
            var nonTxBehavior = new TxBehavior<NonTxRequest, string>(
                _publisherMock.Object,
                _eventContextMock.Object,
                _unitOfWorkMock.Object,
                Mock.Of<ILogger<TxBehavior<NonTxRequest, string>>>()
            );

            var request = new NonTxRequest();
            RequestHandlerDelegate<string> next = (ctx) => Task.FromResult("response");

            var result = await nonTxBehavior.Handle(request, next, default);

            result.Should().Be("response");
            _unitOfWorkMock.Verify(u => u.ExecuteTransactionalAsync(It.IsAny<Func<Task<string>>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Should_Invoke_Transaction_And_Publish_Events()
        {
            var requestMock = new Mock<ITxRequest>();
            var request = requestMock.Object;
            var domainEvent = new SampleDomainEvent();

            _eventContextMock.Setup(e => e.GetDomainEvents())
                             .Returns(new List<BaseEvent> { domainEvent });

            _unitOfWorkMock
                .Setup(u => u.ExecuteTransactionalAsync(It.IsAny<Func<Task<string>>>(), It.IsAny<CancellationToken>()))
                .Returns<Func<Task<string>>, CancellationToken>(async (func, _) => await func());

            RequestHandlerDelegate<string> next = (ctx) => Task.FromResult("response");

            var result = await _txBehavior.Handle(request, next, default);

            result.Should().Be("response");
            _publisherMock.Verify(p => p.Publish(domainEvent, It.IsAny<CancellationToken>()), Times.Never);
        }


        [Fact]
        public async Task Handle_Should_Invoke_Transaction_Without_Events()
        {
            var request = Mock.Of<ITxRequest>();

            _eventContextMock.Setup(e => e.GetDomainEvents()).Returns(new List<BaseEvent>());

            _unitOfWorkMock
                .Setup(u => u.ExecuteTransactionalAsync(It.IsAny<Func<Task<string>>>(), It.IsAny<CancellationToken>()))
                .Returns<Func<Task<string>>, CancellationToken>(async (func, _) => await func());

            RequestHandlerDelegate<string> next = (ctx) => Task.FromResult("response");

            var result = await _txBehavior.Handle(request, next, default);

            result.Should().Be("response");
            _publisherMock.Verify(p => p.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }

}
