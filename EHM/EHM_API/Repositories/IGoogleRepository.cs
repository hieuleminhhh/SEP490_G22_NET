using EHM_API.DTOs.Email;
using EHM_API.Models;

namespace EHM_API.Repositories
{
    public interface IGoogleRepository
    {
        Account GetByEmail(string email);
        Task AddAsync(Account account);
        Account GetAccountById(int accountId);
        void UpdatePassword(int accountId, string newPassword);
        Task<bool> SendEmailAsync(SendEmailRequestDTO emailDto);
    }
}
