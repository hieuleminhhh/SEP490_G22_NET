using EHM_API.DTOs.Email;
using EHM_API.Models;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Options;
namespace EHM_API.Repositories
{
    public class GoogleRepository : IGoogleRepository
    {
        private readonly EHMDBContext _context;
        private readonly EmailSettings _emailSettings;

        public GoogleRepository(EHMDBContext context, IOptions<EmailSettings> emailSettings)
        {
            _context = context;
            _emailSettings = emailSettings.Value;
        }

        public Account GetByEmail(string email)
        {
            return _context.Accounts.FirstOrDefault(a => a.Email == email);
        }

        public async Task AddAsync(Account account)
        {
            await _context.Accounts.AddAsync(account);
            await _context.SaveChangesAsync();
        }
        public Account GetAccountById(int accountId)
        {
            return _context.Accounts.FirstOrDefault(a => a.AccountId == accountId);
        }
        public void UpdatePassword(int accountId, string newPassword)
        {
            var account = GetAccountById(accountId);
            if (account != null)
            {
                account.Password = newPassword;
                _context.SaveChanges();
            }
        }
        public async Task<bool> SendEmailAsync(SendEmailRequestDTO emailDto)
        {
            using (var smtpClient = new SmtpClient(_emailSettings.SMTPHost, _emailSettings.SMTPPort))
            {
                smtpClient.Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password);
                smtpClient.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.From),
                    Subject = emailDto.Subject,
                    Body = emailDto.Body,
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(emailDto.ToEmail);

                try
                {
                    await smtpClient.SendMailAsync(mailMessage);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
