using EHM_API.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace EHM_API.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly EHMDBContext _context;

        public DiscountRepository(EHMDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Discount>> GetAllAsync()
        {
            return await _context.Discounts.ToListAsync();
        }

        public async Task<Discount> GetByIdAsync(int id)
        {
            return await _context.Discounts.FindAsync(id);
        }

        public async Task<Discount> AddAsync(Discount discount)
        {
            _context.Discounts.Add(discount);
            await _context.SaveChangesAsync();
            return discount;
        }

        public async Task<Discount> UpdateAsync(Discount discount)
        {
            _context.Discounts.Update(discount);
            await _context.SaveChangesAsync();
            return discount;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var discount = await _context.Discounts.FindAsync(id);
            if (discount == null)
            {
                return false;
            }

            _context.Discounts.Remove(discount);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Discount>> SearchAsync(string keyword)
        {
            return await _context.Discounts
                .Where(d => d.DiscountName.Contains(keyword) || d.Note.Contains(keyword))
                .ToListAsync();
        }
        public async Task<Discount> GetDiscountByIdAsync(int discountId)
        {
            return await _context.Discounts
                .FirstOrDefaultAsync(d => d.DiscountId == discountId);
        }

        public async Task<int> CountOrdersInRangeAsync(DateTime startTime, DateTime endTime)
        {
            return await _context.Orders
                .CountAsync(o => o.OrderDate >= startTime && o.OrderDate <= endTime);
        }
    }
}