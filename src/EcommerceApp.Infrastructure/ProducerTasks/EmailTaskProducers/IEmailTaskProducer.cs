namespace Infrastructure.ProducerTasks.EmailTaskProducers
{
    public interface IEmailTaskProducer
    {
        void SendEmail(string email, string subject, string body);
    }
}
