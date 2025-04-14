namespace Infrastructure.BackgroundServices.BackgroundTask
{
    public interface IBackgroundTaskQueue
    {
        Task QueueWorkItem(Func<CancellationToken, Task> workItem);

        Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);

        int PendingTaskCount { get; }
    }
}
