using Infrastructure.BackgroundServices.TaskQueues;
using Infrastructure.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.ProducerTasks.CloudTaskProducers
{
    public class CloudTaskProducer : ICloudTaskProducer
    {
        private readonly CloudTaskQueue _taskCloudQueue;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public CloudTaskProducer(CloudTaskQueue taskCloudQueue, IServiceScopeFactory serviceScope)
        {
            _taskCloudQueue = taskCloudQueue;
            _serviceScopeFactory = serviceScope;
        }

        public void AddDeleteImageOnCloudinary(IEnumerable<string> urls)
        {
            _taskCloudQueue.QueueWorkItem(async token =>
            {
                await DeleteImageCloudinary(urls);
            });
        }

        private async Task DeleteImageCloudinary(IEnumerable<string> urls)
        {
            // Get scope
            var scope = _serviceScopeFactory.CreateScope();
            var storageService = scope.ServiceProvider.GetRequiredService<IStorageService>();

            // Delete image
            try
            {
                var result = await storageService.DeleteImages(urls);

                foreach (var item in result)
                {
                    Console.WriteLine($"Image {item.Key} deleted {item.Value}.");
                }
            } catch (Exception ex)
            {
                // Log exception
                Console.WriteLine($"Error deleting image: {ex.Message}");
            } finally
            {
                scope.Dispose();
            }
        }
    }
}
