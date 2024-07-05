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
	}
}
