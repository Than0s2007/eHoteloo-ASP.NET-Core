// Services/EmailSender.cs
using System.Net;
using System.Net.Mail;

namespace eHoteloo.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }

    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;

        public EmailSender(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // لو عندك SMTP حقيقي استخدمه، دي placeholder
            var smtpServer = _config["Email:SmtpServer"];
            var port = int.Parse(_config["Email:Port"]);
            var username = _config["Email:Username"];
            var password = _config["Email:Password"];
            var fromEmail = _config["Email:From"];

            using var client = new SmtpClient(smtpServer, port)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(username, password)
            };

            var message = new MailMessage
            {
                From = new MailAddress(fromEmail),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };
            message.To.Add(email);

            await client.SendMailAsync(message);
        }
    }
}