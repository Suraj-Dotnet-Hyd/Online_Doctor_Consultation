using MimeKit;
using MailKit;
using NotificationService.Models;
using System.Net.Mail;

namespace NotificationService.Service
{
    public class NotificationService : INotificationService
    {
        private readonly IConfiguration _config;

        public NotificationService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<bool> SendEmailAsync(NotificationRequest request)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_config["EmailSettings:From"]));
            email.To.Add(MailboxAddress.Parse(request.ToEmail));
            email.Subject = request.Subject;

            email.Body = new TextPart("html")
            {
                Text = request.Message
            };

            using var smtp = new MailKit.Net.Smtp.SmtpClient(); // <-- Use MailKit's SmtpClient

            await smtp.ConnectAsync(_config["EmailSettings:SmtpServer"],
                                    int.Parse(_config["EmailSettings:Port"]),
                                    true);

            await smtp.AuthenticateAsync(_config["EmailSettings:UserName"],
                                         _config["EmailSettings:Password"]);

            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);

            return true;
        }
    }
}
