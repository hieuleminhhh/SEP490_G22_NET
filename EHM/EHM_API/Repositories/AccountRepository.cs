using EHM_API.Models;
using Microsoft.EntityFrameworkCore;

namespace EHM_API.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly EHMDBContext _context;

        public AccountRepository(EHMDBContext context)
        {
            _context = context;
        }

        public async Task<Account> AddAccountAsync(Account account)
        {
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
            return account;
        }

        public async Task<bool> AccountExistsAsync(string username)
        {
            return await _context.Accounts
                .AnyAsync(a => a.Username == username);
        }
        public async Task<IEnumerable<Account>> GetAllAccountsAsync()
        {
            return await _context.Accounts.ToListAsync();
        }

        public async Task<Account> GetAccountByIdAsync(int id)
        {
            return await _context.Accounts.FindAsync(id);
        }

        public async Task<Account> UpdateAccountAsync(Account account)
        {
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
            return account;
        }

        public async Task<Account> RemoveAccountAsync(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return null;
            }

            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();
            return account;
        }
        public async Task<IEnumerable<Account>> GetAccountsByRoleAsync(string role)
        {
            return await _context.Accounts
                .Where(a => a.Role.ToLower() == role.ToLower() && a.IsActive == true)
                .ToListAsync();
        }
        public async Task<bool> UpdateAccountStatusAsync(int id, bool isActive)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return false;
            }

            account.IsActive = isActive;
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
            return true;
        }

		public async Task<bool> UpdateProfileAccount(Account account)
		{
			_context.Accounts.Update(account);
			return await _context.SaveChangesAsync() > 0;
		}
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
        public Account GetByEmail(string email)
        {
            return _context.Accounts.FirstOrDefault(a => a.Email == email);
        }
        public async Task UpdatePasswordByEmailAsync(string email, string newPassword)
        {
            var account = GetByEmail(email);
            if (account != null)
            {
                // Không mã hóa mật khẩu, lưu trực tiếp mật khẩu thô
                account.Password = newPassword;
                await _context.SaveChangesAsync();
            }
        }
        public async Task<Account> GetAccountByEmailAsync(string email)
        {
            return await _context.Accounts.FirstOrDefaultAsync(a => a.Email == email);
        }

    }
}
