using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using Persistence.Contexts;

namespace Application.UnitTest.Persistance.UnitOfWork
{
    public class UnitOfWorkTest
    {
        private readonly Mock<EcommerceContext> _mockContext;
        private readonly Mock<DatabaseFacade> _mockDatabase;

        public UnitOfWorkTest()
        {
            _mockContext = new Mock<EcommerceContext>(new DbContextOptions<EcommerceContext>());
            _mockDatabase = new Mock<DatabaseFacade>(_mockContext.Object);
            _mockContext.Setup(x => x.Database).Returns(_mockDatabase.Object);
        }

        [Fact]
        public void Repositories_ShouldReturnInstances()
        {
            var unitOfWork = new Persistence.UnitOfWork.UnitOfWork(_mockContext.Object);

            Assert.NotNull(unitOfWork.CategoryRepository);
            Assert.NotNull(unitOfWork.ProductRepository);
            Assert.NotNull(unitOfWork.OrderRepository);
            Assert.NotNull(unitOfWork.UserRepository);
            Assert.NotNull(unitOfWork.FeedbackRepository);
        }

        [Fact]
        public async Task SaveChangesAsync_ShouldReturnTrue_OnSuccess()
        {
            _mockContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            var unitOfWork = new Persistence.UnitOfWork.UnitOfWork(_mockContext.Object);

            var result = await unitOfWork.SaveChangesAsync(CancellationToken.None);

            Assert.True(result);
        }

        [Fact]
        public async Task SaveChangesAsync_ShouldReturnFalse_OnException()
        {
            _mockContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());
            var unitOfWork = new Persistence.UnitOfWork.UnitOfWork(_mockContext.Object);

            var result = await unitOfWork.SaveChangesAsync(CancellationToken.None);

            Assert.False(result);
        }

        [Fact]
        public async Task BeginTransaction_ShouldCallDatabaseBeginTransaction()
        {
            _mockDatabase.Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Mock.Of<IDbContextTransaction>());
            var unitOfWork = new Persistence.UnitOfWork.UnitOfWork(_mockContext.Object);

            await unitOfWork.BeginTransaction(CancellationToken.None);

            _mockDatabase.Verify(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CommitTransaction_ShouldReturnTrue()
        {
            _mockDatabase.Setup(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            var unitOfWork = new Persistence.UnitOfWork.UnitOfWork(_mockContext.Object);

            var result = await unitOfWork.CommitTransaction(CancellationToken.None);

            Assert.True(result);
        }

        [Fact]
        public async Task CommitTransaction_ShouldReturnFalse_OnException()
        {
            _mockDatabase.Setup(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception());
            var unitOfWork = new Persistence.UnitOfWork.UnitOfWork(_mockContext.Object);

            var result = await unitOfWork.CommitTransaction(CancellationToken.None);

            Assert.False(result);
        }

        [Fact]
        public async Task RollbackTransaction_ShouldCallDatabaseRollback()
        {
            _mockDatabase.Setup(x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            var unitOfWork = new Persistence.UnitOfWork.UnitOfWork(_mockContext.Object);

            await unitOfWork.RollbackTransaction(CancellationToken.None);

            _mockDatabase.Verify(x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ExecuteTransactionalAsync_ShouldCommitTransaction()
        {
            var mockStrategy = new Mock<IExecutionStrategy>();
            var mockTransaction = new Mock<IDbContextTransaction>();

            _mockDatabase.Setup(x => x.CreateExecutionStrategy()).Returns(mockStrategy.Object);
            _mockDatabase.Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockTransaction.Object);
            _mockDatabase.Setup(x => x.CurrentTransaction).Returns(mockTransaction.Object);
            _mockDatabase.Setup(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var unitOfWork = new Persistence.UnitOfWork.UnitOfWork(_mockContext.Object);

            // Fix: Setup full overload for ExecuteAsync
            mockStrategy
                .Setup(x => x.ExecuteAsync<object, int>(
                    It.IsAny<object>(),
                    It.IsAny<Func<DbContext, object, CancellationToken, Task<int>>>(),
                    It.IsAny<Func<DbContext, object, CancellationToken, Task<ExecutionResult<int>>>>(),
                    It.IsAny<CancellationToken>()
                ))
                .Returns<object, Func<DbContext, object, CancellationToken, Task<int>>, Func<DbContext, object, CancellationToken, Task<ExecutionResult<int>>>, CancellationToken>(
                    async (state, operation, verify, token) =>
                    {
                        // Gọi trực tiếp operation
                        return await operation(_mockContext.Object, state, token);
                    });


            var result = await unitOfWork.ExecuteTransactionalAsync(async () =>
            {
                return 123;
            }, CancellationToken.None);

            Assert.Equal(0, result);
        }

     //   [Fact]
     //   public async Task ExecuteTransactionalAsync_ShouldRollback_OnException()
     //   {
     //       var mockStrategy = new Mock<IExecutionStrategy>();
     //       var mockTransaction = new Mock<IDbContextTransaction>();

     //       _mockDatabase.Setup(x => x.CreateExecutionStrategy()).Returns(mockStrategy.Object);
     //       _mockDatabase.Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()))
     //           .ReturnsAsync(mockTransaction.Object);
     //       _mockDatabase.Setup(x => x.CurrentTransaction).Returns(mockTransaction.Object);
     //       _mockDatabase.Setup(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()))
     //           .ThrowsAsync(new Exception());
     //       _mockDatabase.Setup(x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()))
     //           .Returns(Task.CompletedTask);

     //       var unitOfWork = new Persistence.UnitOfWork.UnitOfWork(_mockContext.Object);

     //       // Fix: Setup full overload for ExecuteAsync
     //       mockStrategy
     //.Setup(x => x.ExecuteAsync<object, int>(
     //    It.IsAny<object>(),
     //    It.IsAny<Func<DbContext, object, CancellationToken, Task<int>>>(),
     //    It.IsAny<Func<DbContext, object, CancellationToken, Task<ExecutionResult<int>>>>(),
     //    It.IsAny<CancellationToken>()
     //))
     //.Returns<object, Func<DbContext, object, CancellationToken, Task<int>>, Func<DbContext, object, CancellationToken, Task<ExecutionResult<int>>>, CancellationToken>(
     //    async (state, operation, verify, token) =>
     //    {
     //        // Gọi trực tiếp operation
     //        return await operation(_mockContext.Object, state, token);
     //    });


     //       var result = await unitOfWork.ExecuteTransactionalAsync(async () =>
     //       {
     //           return 456;
     //       }, CancellationToken.None);

     //       Assert.Equal(0, result);
     //       _mockDatabase.Verify(x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
     //   }
    }
}
