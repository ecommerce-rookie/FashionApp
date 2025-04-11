using Infrastructure.Email.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Email
{
    public static class MailConfig
    {
        public static void AddFluentEmail(this IServiceCollection services, IConfiguration configuration)
        {
            var mailSetting = configuration.GetSection(nameof(MailSetting)).Get<MailSetting>();

            services.AddFluentEmail(mailSetting!.Mail)
            .AddMailKitSender(new FluentEmail.MailKitSmtp.SmtpClientOptions
            {
                Server = mailSetting.SmtpServer,
                Port = mailSetting.Port,
                UseSsl = false,
                RequiresAuthentication = true,
                Password = mailSetting.Password,
                User = mailSetting.Mail

            });
        }
    }
}
