using System.Threading.Tasks;
using EHM_API.Models;
using Microsoft.EntityFrameworkCore;

namespace EHM_API.Repositories
{
    public class SettingRepository : ISettingRepository
    {
        private readonly EHMDBContext _context;

        public SettingRepository(EHMDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Setting>> GetAllAsync()
        {
            return await _context.Settings.ToListAsync();
        }

        public async Task<Setting> AddAsync(Setting setting)
        {
            _context.Settings.Add(setting);
            await _context.SaveChangesAsync();
            return setting;
        }
        public async Task<Setting> GetByIdAsync(int id)
        {
            return await _context.Settings.FindAsync(id);
        }

        public async Task<Setting> UpdateAsync(Setting setting)
        {
            var existingSetting = await _context.Settings.FindAsync(setting.Id);
            if (existingSetting == null)
            {
                return null;
            }

            _context.Entry(existingSetting).CurrentValues.SetValues(setting);
            await _context.SaveChangesAsync();
            return existingSetting;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var setting = await _context.Settings.FindAsync(id);
            if (setting == null)
            {
                return false;
            }

            _context.Settings.Remove(setting);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
