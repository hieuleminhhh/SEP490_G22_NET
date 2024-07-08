using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EHM_API.DTOs.OrderDetailDTO.Manager;
using EHM_API.Models;
using Microsoft.EntityFrameworkCore;

namespace EHM_API.Repositories
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        private readonly EHMDBContext _context;
        private readonly IMapper _mapper;

        public OrderDetailRepository(EHMDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OrderDetailForChefDTO>> GetOrderDetailsAsync()
        {
            var orderDetails = await _context.OrderDetails
                .Include(od => od.Dish)
                .Include(od => od.Combo)
                .ThenInclude(c => c.ComboDetails)
                .ThenInclude(cd => cd.Dish)
                .ToListAsync();

            return _mapper.Map<IEnumerable<OrderDetailForChefDTO>>(orderDetails);
        }
        public async Task<IEnumerable<OrderDetailForChefDTO>> GetOrderDetailSummaryAsync()
        {
            var orderDetails = await _context.OrderDetails
                .Include(od => od.Dish)
                .Include(od => od.Combo)
                .ThenInclude(c => c.ComboDetails)
                .ThenInclude(cd => cd.Dish)
                .ToListAsync();

            var result = orderDetails
                .GroupBy(od => new { od.DishId, od.ComboId })
                .Select(g => new OrderDetailForChefDTO
                {
                    ItemName = g.First().Dish?.ItemName ?? "",
                    ComboName = g.First().Combo?.NameCombo ?? "",
                    ItemInComboName = g.First().Combo != null ?
                        string.Join(", ", g.First().Combo.ComboDetails.Select(cd => cd.Dish.ItemName)) : "",
                    Quantity = (int)g.Sum(od => od.Quantity)
                });

            return result.ToList();
        }
        public async Task UpdateDishesServedAsync(List<int> orderDetailIds)
        {
            var orderDetails = await _context.OrderDetails
                .Where(od => orderDetailIds.Contains(od.OrderDetailId))
                .ToListAsync();

            foreach (var orderDetail in orderDetails)
            {
                orderDetail.DishesServed = 1; 
            }

            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<OrderDetail>> GetOrderDetailsByDishesServedAsync(int? dishesServed)
        {
            return await _context.Set<OrderDetail>()
                                 .Include(od => od.Dish)
                                 .Include(od => od.Combo)
                                 .ThenInclude(c => c.ComboDetails)
                                 .ThenInclude(cd => cd.Dish)
                                 .Where(od => !dishesServed.HasValue || od.DishesServed == dishesServed)
                                 .ToListAsync();
        }
    }
}
