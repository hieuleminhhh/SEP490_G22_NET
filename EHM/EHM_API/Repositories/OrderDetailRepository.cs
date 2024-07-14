using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EHM_API.DTOs.OrderDetailDTO.Manager;
using EHM_API.DTOs.OrderDTO.Manager;
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
                .OrderBy(od => od.OrderDishDate)
                .ToListAsync();

            var orderDetailDTOs = _mapper.Map<IEnumerable<OrderDetailForChefDTO>>(orderDetails);

            return orderDetailDTOs;
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
                    ItemName = g.First().Dish?.ItemName,
                    Quantity = g.Sum(od => od.Quantity),
                    OrderDishDate = g.First().OrderDishDate,
                    Note = g.First().Note,
                    DishesServed = g.Sum(od => od.DishesServed),
                    ComboDetailsForChef = g.First().Combo != null ? new List<ComboDetailForChefDTO>
                    {
                new ComboDetailForChefDTO
                {
                    ComboName = g.First().Combo.NameCombo,                   
                    ItemsInCombo = g.First().Combo.ComboDetails.Select(cd => new ItemInComboDTO
                    {
                        ItemName = cd.Dish.ItemName,
                        Quantity = cd.Quantity
                    }).ToList(),
                    Note = g.First().Combo.Note,
                    OrderDishDate = g.First().OrderDishDate
                }
                    } : new List<ComboDetailForChefDTO>()
                });

            return result.ToList();
        }

        public async Task UpdateDishesServedAsync(int orderDetailId, int? dishesServed)
        {
            var orderDetail = await _context.OrderDetails
                .FirstOrDefaultAsync(od => od.OrderDetailId == orderDetailId);

            if (orderDetail != null)
            {
                if (dishesServed <= orderDetail.Quantity)
                {
                    orderDetail.DishesServed = dishesServed;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new InvalidOperationException("DishesServed cannot be greater than Quantity.");
                }
            }
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
