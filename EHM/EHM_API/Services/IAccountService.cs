using EHM_API.DTOs.AccountDTO;
using EHM_API.Models;

namespace EHM_API.Services
{
	public interface IAccountService
	{
		Task<Account> CreateAccountAsync(CreateAccountDTO accountDTO);

		Task<bool> AccountExistsAsync(string username);
	}
}
