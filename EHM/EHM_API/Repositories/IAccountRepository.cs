using EHM_API.Models;

namespace EHM_API.Repositories
{
    public interface IAccountRepository
    {
        Task<Account> AddAccountAsync(Account account);

        Task<bool> AccountExistsAsync(string username);
        Task<IEnumerable<Account>> GetAllAccountsAsync();
        Task<Account> GetAccountByIdAsync(int id);
        Task<Account> UpdateAccountAsync(Account account);
        Task<Account> RemoveAccountAsync(int id);
        Task<IEnumerable<Account>> GetAccountsByRoleAsync(string role);
        Task<bool> UpdateAccountStatusAsync(int id, bool isActive);
        Task<bool> UpdateProfileAccount(Account account);
        Task SaveAsync();
        Task UpdatePasswordByEmailAsync(string email, string newPassword);
        Account GetByEmail(string email);
        Task<Account> GetAccountByEmailAsync(string email);
    }
}
