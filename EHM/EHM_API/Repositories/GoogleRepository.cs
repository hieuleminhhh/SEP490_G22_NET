using EHM_API.Models;

namespace EHM_API.Repositories
{
    public class GoogleRepository : IGoogleRepository
    {
        private readonly EHMDBContext _context;

        public GoogleRepository(EHMDBContext context)
        {
            _context = context;
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
    }
}
