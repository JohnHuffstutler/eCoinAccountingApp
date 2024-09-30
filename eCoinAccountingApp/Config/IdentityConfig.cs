using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;

namespace eCoinAccountingApp
{
    public class EmailService : IEmailSender
    {
        private readonly EmailServiceCredentials _emailServiceCredentials;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailServiceCredentials = new EmailServiceCredentials(emailSettings);
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
#if !DEBUG
            return SendEmailAsyncInternal(email, subject, htmlMessage);
#else
            return Task.CompletedTask;
#endif
        }

        private Task SendEmailAsyncInternal(string email, string subject, string message)
        {
            var mailMessage = GenerateMailMessage(email, subject, message);

            if (mailMessage != null)
            {
                return GetSmtpClient().SendMailAsync(mailMessage);
            }
            return Task.CompletedTask;
        }

        private SmtpClient GetSmtpClient()
        {
            return new SmtpClient(_emailServiceCredentials.EmailSMTPUrl)
            {
                Port = int.Parse(_emailServiceCredentials.PortNumber),
                EnableSsl = true,
                Credentials = new NetworkCredential(_emailServiceCredentials.EmailFromAddress, _emailServiceCredentials.EmailSMTPPasswordHash)
            };
        }

        private MailMessage GenerateMailMessage(string destination, string subject, string body)
        {
            return new MailMessage(new MailAddress(_emailServiceCredentials.EmailFromAddress, _emailServiceCredentials.EmailFromName), new MailAddress(destination))
            {
                Subject = $"{_emailServiceCredentials.EmailAppName} - {subject}",
                Body = body,
                IsBodyHtml = true
            };
        }
    }
}
