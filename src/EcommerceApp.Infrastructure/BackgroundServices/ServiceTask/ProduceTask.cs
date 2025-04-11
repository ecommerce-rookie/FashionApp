using Infrastructure.BackgroundServices.BackgroundTask;

namespace Infrastructure.BackgroundServices.ServiceTask
{
    public class ProduceTask
    {
        private readonly IBackgroundTaskQueue _taskQueue;

        public ProduceTask(IBackgroundTaskQueue taskQueue)
        {
            _taskQueue = taskQueue;
        }

        private void PublishAssignmentNotification(Guid assignmentId)
        {

        }
    }
}
