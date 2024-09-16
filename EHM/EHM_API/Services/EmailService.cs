using MimeKit;
using MailKit.Net.Smtp; 
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using EHM_API.DTOs.Email;

namespace EHM_API.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_emailSettings.From, _emailSettings.From));
            email.To.Add(new MailboxAddress(toEmail, toEmail));
            email.Subject = subject;

            var builder = new BodyBuilder
            {
                HtmlBody = body
            };
            email.Body = builder.ToMessageBody();

            // Sử dụng SmtpClient của MailKit
            using (var smtp = new SmtpClient())
            {
                await smtp.ConnectAsync(_emailSettings.SMTPHost, _emailSettings.SMTPPort, MailKit.Security.SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
        }
    }
}
