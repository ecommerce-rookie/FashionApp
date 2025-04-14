using Infrastructure.BackgroundServices.TaskQueues;

namespace Infrastructure.ProducerTasks.EmailTaskProducers
{
    public class EmailTaskProducer : IEmailTaskProducer
    {
        private readonly EmailTaskQueue _taskEmailQueue;

        public EmailTaskProducer(EmailTaskQueue taskEmailQueue)
        {
            _taskEmailQueue = taskEmailQueue;
        }

        public void SendEmail(string email, string subject, string body)
        {
            _taskEmailQueue.QueueWorkItem(async token =>
            {
                await SendEmailAsync(email, subject, body);
            });
        }

        private async Task SendEmailAsync(string email, string subject, string body)
        {
            // Simulate sending email
            await Task.Delay(5000);

            Console.WriteLine($"Email sent to {email} with subject: {subject}");
        }

    }
}
