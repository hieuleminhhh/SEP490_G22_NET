using EHM_API.Models;

namespace EHM_API.Repositories
{
    public interface IGoogleRepository
    {
        Account GetByEmail(string email);
        Task AddAsync(Account account);
    }
}
