namespace Infrastructure.ProducerTasks.CloudTaskProducers
{
    public interface ICloudTaskProducer
    {
        void AddDeleteImageOnCloudinary(IEnumerable<string> urls);
    }
}
