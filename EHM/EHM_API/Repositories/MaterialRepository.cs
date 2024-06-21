using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EHM_API.Models;

namespace EHM_API.Repositories
{
    public class MaterialRepository : IMaterialRepository
    {
        private readonly EHMDBContext _context;

        public MaterialRepository(EHMDBContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Material>> GetAllAsync()
        {
            return await _context.Materials.ToListAsync();
        }
        public async Task<Material> GetByIdAsync(int id)
        {
            return await _context.Materials.FindAsync(id);
        }

        public async Task<IEnumerable<Material>> SearchByNameAsync(string name)
        {
            return await _context.Materials
                .Where(m => m.Name.Equals(name))
                .ToListAsync();
        }

        public async Task<Material> CreateAsync(Material material)
        {
            _context.Materials.Add(material);
            await _context.SaveChangesAsync();
            return material;
        }

        public async Task<Material> UpdateAsync(Material material)
        {
            _context.Materials.Update(material);
            await _context.SaveChangesAsync();
            return material;
        }
        public async Task DeleteIngredientsByMaterialIDAsync(int materialId)
        {
            var sql = "DELETE FROM Ingredient WHERE MaterialID = @p0";
            await _context.Database.ExecuteSqlRawAsync(sql, materialId);
        }

        public async Task DeleteAsync(int id)
        {
            await DeleteIngredientsByMaterialIDAsync(id);

            var material = await _context.Materials.FindAsync(id);
            if (material != null)
            {
                _context.Materials.Remove(material);
                await _context.SaveChangesAsync();
            }
        }
    }
}
