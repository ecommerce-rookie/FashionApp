using Infrastructure.Email.Settings;

namespace Infrastructure.Email
{
    public interface IEmailService<T>
    {
        Task<bool> SendEmail(MailMetadata metadata, MailModel<T> mailDto);
        Task<IDictionary<string, bool>> SendEmails(MailMetadata metadata, IEnumerable<MailModel<T>> mailDtos);
    }
}
