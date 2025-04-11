using FluentEmail.Core;
using Infrastructure.Email.Settings;

namespace Infrastructure.Email.Mailkit
{
    public class MailkitService<T> : IEmailService<T> where T : class
    {
        private readonly IFluentEmail _fluentEmail;
        private readonly int _batchSize = 3;

        public MailkitService(IFluentEmail fluentEmail)
        {
            _fluentEmail = fluentEmail;
        }

        public async Task<bool> SendEmail(MailMetadata metadata, MailModel<T> mailDto)
        {
            var renderedTemplate = mailDto.TemplatePath != null ? mailDto.TemplatePath
                .Replace("@Model.UserName", mailDto.Model.ToString())
                : string.Empty;

            var response = await _fluentEmail.To(mailDto.To)
                                            .Subject(metadata.Subject)
                                            .Body(renderedTemplate, isHtml: metadata.IsHtml)
                                            .SendAsync();

            if (response.Successful)
            {
                Console.WriteLine($"Email sent to {mailDto.To}: {response}");
                return true;
            }

            Console.WriteLine($"Failed to send email to {mailDto.To}: {string.Join(", ", response.ErrorMessages)}");
            return false;
        }

        public async Task<IDictionary<string, bool>> SendEmails(MailMetadata metadata, IEnumerable<MailModel<T>> mailDtos)
        {
            var result = new Dictionary<string, bool>();

            // Batch processing
            var mailBatches = mailDtos
                .Select((mailDto, index) => new { mailDto, index })
                .GroupBy(x => x.index / _batchSize)
                .Select(g => g.Select(x => x.mailDto).ToList())
                .ToList();

            foreach (var batch in mailBatches)
            {
                var batchTasks = batch.Select(mailDto => SendEmail(metadata, mailDto));
                var batchResults = await Task.WhenAll(batchTasks);

                // Process results to add to the dictionary
                for(int i = 0; i < batchResults.Length; i++)
                {
                    result.Add(batch[i].To, batchResults[i]);

                    Console.WriteLine($"Email sent to {batch[i].To}: {batchResults[i]}");
                }
            }

            return result;
        }
    }
}
