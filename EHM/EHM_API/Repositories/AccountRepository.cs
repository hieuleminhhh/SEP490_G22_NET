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
    }
}
