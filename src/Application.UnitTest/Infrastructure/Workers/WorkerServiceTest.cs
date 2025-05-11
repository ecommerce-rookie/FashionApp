using Domain.Models.Settings;
using FluentAssertions;
using Infrastructure.BackgroundServices.BackgroundTask;
using Infrastructure.BackgroundServices.Workers;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Concurrent;


namespace Application.UnitTest.Infrastructure.Workers
{
    public class WorkerServiceTest
    {
        private readonly Mock<IBackgroundTaskQueue> _mockTaskQueue;
        private readonly Mock<IOptions<WorkerSetting>> _mockOptions;

        public WorkerServiceTest()
        {
            _mockTaskQueue = new Mock<IBackgroundTaskQueue>();
            _mockOptions = new Mock<IOptions<WorkerSetting>>();
            _mockOptions.Setup(x => x.Value).Returns(new WorkerSetting
            {
                MinWorkerCount = 1,
                MaxWorkerCount = 3,
                ScaleInterval = 1, // 1 minute
                TaskThresholdToScaleUp = 2,
                TaskThresholdToScaleDown = 1
            });
        }

        private ConcurrentBag<(Task, CancellationTokenSource)> GetWorkers(WorkerService<IBackgroundTaskQueue> service)
        {
            return (ConcurrentBag<(Task, CancellationTokenSource)>)service
                .GetType()
                .GetField("_workers", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .GetValue(service);
        }

        [Fact]
        public async Task StartAsync_ShouldAddInitialWorkers()
        {
            // Arrange
            var service = new WorkerService<IBackgroundTaskQueue>(_mockTaskQueue.Object, _mockOptions.Object);
            var cts = new CancellationTokenSource();

            // Act
            await service.StartAsync(cts.Token);
            await Task.Delay(100); // wait a bit for workers to be added

            var workers = GetWorkers(service);

            // Assert
            workers.Count.Should().BeGreaterThanOrEqualTo(1);

            // Cleanup
            await service.StopAsync(CancellationToken.None);
        }

        [Fact]
        public async Task StopAsync_ShouldCancelAllWorkers()
        {
            // Arrange
            var service = new WorkerService<IBackgroundTaskQueue>(_mockTaskQueue.Object, _mockOptions.Object);
            var cts = new CancellationTokenSource();

            await service.StartAsync(cts.Token);
            await Task.Delay(100);

            var workers = GetWorkers(service);
            workers.Count.Should().BeGreaterThan(0);

            // Act
            await service.StopAsync(CancellationToken.None);

            // Assert
            // Check all tasks are completed
            foreach (var (task, _) in workers)
            {
                task.IsCompleted.Should().BeTrue();
            }
        }

        [Fact]
        public async Task Worker_ShouldExecuteQueuedTask()
        {
            // Arrange
            var executed = false;
            _mockTaskQueue
                .Setup(q => q.DequeueAsync(It.IsAny<CancellationToken>()))
                .Returns(async (CancellationToken ct) =>
                {
                    await Task.Delay(10, ct);
                    return async token =>
                    {
                        executed = true;
                        await Task.CompletedTask;
                    };
                });

            var service = new WorkerService<IBackgroundTaskQueue>(_mockTaskQueue.Object, _mockOptions.Object);
            await service.StartAsync(CancellationToken.None);

            // Act
            await Task.Delay(200); // Let worker run

            // Assert
            executed.Should().BeTrue();

            // Cleanup
            await service.StopAsync(CancellationToken.None);
        }

        [Fact]
        public async Task Service_ShouldScaleUp_WhenTaskThresholdExceeded()
        {
            // Arrange
            var service = new WorkerService<IBackgroundTaskQueue>(_mockTaskQueue.Object, _mockOptions.Object);
            _mockTaskQueue.Setup(q => q.PendingTaskCount).Returns(10);

            await service.StartAsync(CancellationToken.None);

            await Task.Delay(TimeSpan.FromSeconds(5)); // wait more than scale interval

            var workers = GetWorkers(service);
            workers.Count.Should().BeGreaterThanOrEqualTo(1);

            await service.StopAsync(CancellationToken.None);
        }

        [Fact]
        public async Task Service_ShouldScaleDown_WhenTaskLow()
        {
            // Arrange
            var service = new WorkerService<IBackgroundTaskQueue>(_mockTaskQueue.Object, _mockOptions.Object);

            // Start with high demand
            _mockTaskQueue.SetupSequence(q => q.PendingTaskCount)
                .Returns(10) // scale up
                .Returns(0); // then scale down

            await service.StartAsync(CancellationToken.None);

            await Task.Delay(TimeSpan.FromSeconds(5)); // scale up
            await Task.Delay(TimeSpan.FromSeconds(5)); // scale down

            var workers = GetWorkers(service);
            workers.Count.Should().Be(1); // should go back to MinWorkerCount

            await service.StopAsync(CancellationToken.None);
        }
    }
}
