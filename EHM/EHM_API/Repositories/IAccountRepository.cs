using EHM_API.Models;

namespace EHM_API.Repositories
{
	public interface IAccountRepository
	{
		Task<Account> AddAccountAsync(Account account);

		Task<bool> AccountExistsAsync(string username);
	}
}
