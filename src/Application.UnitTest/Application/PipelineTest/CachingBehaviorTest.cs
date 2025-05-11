using Application.Behaviors;
using Domain.Models.Common;
using FluentAssertions;
using Infrastructure.Cache;
using Infrastructure.Cache.Attributes;
using MediatR;
using Moq;
using System.Net;

namespace Application.UnitTest.Application.PipelineTest
{
    public class TestRequest : IRequest<APIResponse>
    {
        public string Value { get; set; } = "Test";
    }

    [Cache("test", 60)]
    public class CachedTestRequest : IRequest<APIResponse>
    {
        public string Value { get; set; } = "Test";
    }


    [Cache("test", 60)]
    public class CachedListRequest : IRequest<IEnumerable<string>>
    {
    }

    public class CachingBehaviorTest
    {
        private readonly Mock<ICacheService> _cacheMock;
        private readonly CachingBehavior<IRequest<APIResponse>, APIResponse> _sut;

        public CachingBehaviorTest()
        {
            _cacheMock = new Mock<ICacheService>();
            _sut = new CachingBehavior<IRequest<APIResponse>, APIResponse>(_cacheMock.Object);
        }

        [Fact]
        public async Task Handle_Should_Proceed_Without_Cache_When_No_Attribute()
        {
            // Arrange  
            var request = new TestRequest();
            var expected = new APIResponse { Status = HttpStatusCode.OK, Data = "OK" };
            var handler = new RequestHandlerDelegate<APIResponse>((ct) => Task.FromResult(expected));

            // Act  
            var result = await _sut.Handle(request, handler, CancellationToken.None);

            // Assert  
            result.Should().BeEquivalentTo(expected);
            _cacheMock.Verify(x => x.GetAsync<APIResponse>(It.IsAny<string>()), Times.Never);
            _cacheMock.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<APIResponse>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Should_Return_Cached_Response_When_Available()
        {
            // Arrange  
            var request = new CachedTestRequest();
            var expected = new APIResponse { Status = HttpStatusCode.OK, Data = "OK" };
            _cacheMock.Setup(x => x.GetAsync<APIResponse>(It.IsAny<string>())).ReturnsAsync(expected);

            var handler = new RequestHandlerDelegate<APIResponse>((ct) => throw new Exception("Should not call next"));

            var behavior = new CachingBehavior<CachedTestRequest, APIResponse>(_cacheMock.Object);

            // Act  
            var result = await behavior.Handle(request, handler, CancellationToken.None);

            // Assert  
            result.Should().BeEquivalentTo(expected);
            _cacheMock.Verify(x => x.GetAsync<APIResponse>(It.IsAny<string>()), Times.Once);
            _cacheMock.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<APIResponse>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Should_Set_Cache_When_No_Cache_And_Valid_APIResponse()
        {
            // Arrange  
            var request = new CachedTestRequest();
            var expected = new APIResponse { Status = HttpStatusCode.OK, Data = "OK" };
            _cacheMock.Setup(x => x.GetAsync<APIResponse>(It.IsAny<string>())).ReturnsAsync((APIResponse?)null);

            var handler = new RequestHandlerDelegate<APIResponse>((ct) => Task.FromResult(expected));
            var behavior = new CachingBehavior<CachedTestRequest, APIResponse>(_cacheMock.Object);

            // Act  
            var result = await behavior.Handle(request, handler, CancellationToken.None);

            // Assert  
            result.Should().BeEquivalentTo(expected);
            _cacheMock.Verify(x => x.SetAsync("test", It.IsAny<string>(), expected, 60), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Not_Set_Cache_When_APIResponse_Is_Not_Success()
        {
            // Arrange  
            var request = new CachedTestRequest();
            var expected = new APIResponse { Status = HttpStatusCode.BadRequest, Data = "OK" };
            _cacheMock.Setup(x => x.GetAsync<APIResponse>(It.IsAny<string>())).ReturnsAsync((APIResponse?)null);

            var handler = new RequestHandlerDelegate<APIResponse>((ct) => Task.FromResult(expected));
            var behavior = new CachingBehavior<CachedTestRequest, APIResponse>(_cacheMock.Object);

            // Act  
            var result = await behavior.Handle(request, handler, CancellationToken.None);

            // Assert  
            result.Should().BeEquivalentTo(expected);
            _cacheMock.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<APIResponse>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Should_Return_Cached_Response_If_Exists()
        {
            // Arrange
            var expected = new List<string> { "cached" };
            _cacheMock.Setup(c => c.GetAsync<IEnumerable<string>>(It.IsAny<string>())).ReturnsAsync(expected);

            var handler = new RequestHandlerDelegate<IEnumerable<string>>((ctx) => throw new Exception("Should not call next"));
            var behavior = new CachingBehavior<CachedListRequest, IEnumerable<string>>(_cacheMock.Object);

            // Act
            var result = await behavior.Handle(new CachedListRequest(), handler, default);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task Handle_Should_Invoke_Next_If_Cache_Miss_And_Set_Cache_When_Enumerable_Has_Data()
        {
            // Arrange
            var data = new List<string> { "a", "b" };
            _cacheMock.Setup(c => c.GetAsync<IEnumerable<string>>(It.IsAny<string>())).ReturnsAsync((IEnumerable<string>?)null);

            var handler = new RequestHandlerDelegate<IEnumerable<string>>((ct) => Task.FromResult((IEnumerable<string>)data));
            var behavior = new CachingBehavior<CachedListRequest, IEnumerable<string>>(_cacheMock.Object);

            // Act
            var result = await behavior.Handle(new CachedListRequest(), handler, default);

            // Assert
            result.Should().BeEquivalentTo(data);
            _cacheMock.Verify(c =>
                c.SetAsync("test", It.IsAny<string>(), It.Is<IEnumerable<string>>(d => d.SequenceEqual(data)), 60),
                Times.Once);

        }

        [Fact]
        public async Task Handle_Should_Not_Set_Cache_When_Enumerable_Is_Empty()
        {
            // Arrange
            var data = Enumerable.Empty<string>();
            _cacheMock.Setup(c => c.GetAsync<IEnumerable<string>>(It.IsAny<string>())).ReturnsAsync((IEnumerable<string>?)null);

            var handler = new RequestHandlerDelegate<IEnumerable<string>>((ct) => Task.FromResult((IEnumerable<string>)data));
            var behavior = new CachingBehavior<CachedListRequest, IEnumerable<string>>(_cacheMock.Object);

            // Act
            var result = await behavior.Handle(new CachedListRequest(), handler, default);

            // Assert
            result.Should().BeEmpty();
            _cacheMock.Verify(c => c.SetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<int>()), Times.Never);
        }

        public class NoCacheRequest : IRequest<string> { }

        [Fact]
        public async Task Handle_Should_Just_Invoke_Next_When_No_CacheAttribute()
        {
            // Arrange  
            var response = "not cached";
            var handler = new RequestHandlerDelegate<string>((ct) => Task.FromResult(response));
            var behavior = new CachingBehavior<NoCacheRequest, string>(_cacheMock.Object);

            // Act  
            var result = await behavior.Handle(new NoCacheRequest(), handler, default);

            // Assert  
            result.Should().Be(response);
            _cacheMock.Verify(c => c.GetAsync<string>(It.IsAny<string>()), Times.Never);
            _cacheMock.Verify(c => c.SetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        }

    }
}
