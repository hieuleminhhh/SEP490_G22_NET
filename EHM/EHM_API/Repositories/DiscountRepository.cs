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

        public async Task<Discount> GetByIdAsync(int discountId)
        {
          
            var discount = await _context.Discounts
                .FirstOrDefaultAsync(d => d.DiscountId == discountId);

            if (discount == null)
            {
                return null;
            }

            
            if (discount.Type == 2)
            {
               
                await _context.Entry(discount)
                    .Collection(d => d.Dishes)
                    .LoadAsync();
            }

            return discount;
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

        public async Task<IEnumerable<Discount>> GetActiveDiscountsAsync()
        {
            return await _context.Discounts
                .Where(d => d.DiscountStatus == true && d.Type == 1)
                .ToListAsync();
        }
        public async Task<IEnumerable<Discount>> GetDiscountsWithSimilarAttributesAsync(int discountId)
        {
            var discount = await GetByIdAsync(discountId);
            if (discount == null)
            {
                return Enumerable.Empty<Discount>();
            }

            return await _context.Discounts
                .Where(d => d.DiscountId != discountId &&
                            d.DiscountStatus == discount.DiscountStatus &&
                            d.DiscountName == discount.DiscountName &&
                            d.Type == discount.Type &&
                            d.StartTime == discount.StartTime &&
                            d.EndTime == discount.EndTime &&
                            d.Note == discount.Note)
                .ToListAsync();
        }

		public async Task<Discount> GetDiscountByOrderIdAsync(int orderId)
		{
			return await _context.Orders
				.Where(o => o.OrderId == orderId)
				.Select(o => o.Discount)
				.FirstOrDefaultAsync();
		}

		public async Task<bool> DiscountNameExistsAsync(string discountName)
		{
			return await _context.Discounts.AnyAsync(d => d.DiscountName == discountName);
		}
	}
}