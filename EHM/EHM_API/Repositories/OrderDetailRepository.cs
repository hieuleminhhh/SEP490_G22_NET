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

        public async Task<IEnumerable<OrderForChefDTO>> GetOrderDetailsAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Dish)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Combo)
                    .ThenInclude(c => c.ComboDetails)
                    .ThenInclude(cd => cd.Dish)
                    .OrderBy(o => o.OrderDate)
                .Where(o => (o.Type == 1 || o.Type == 4) && o.Status == 2)
                .ToListAsync();

            var orderForChefDTOs = orders.Select(o => new OrderForChefDTO
            {
                OrderId = o.OrderId,
                OrderDate = o.OrderDate,
                Status = o.Status,
                Type = o.Type,
                OrderDetails = _mapper.Map<IEnumerable<OrderDetailForChefDTO>>(o.OrderDetails)
            });

            return orderForChefDTOs;
        }
        public async Task<IEnumerable<OrderForChef1DTO>> GetOrderDetails1Async()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Dish)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Combo)
                    .ThenInclude(c => c.ComboDetails)
                    .ThenInclude(cd => cd.Dish)
                    .OrderBy(o => o.RecevingOrder)
                .Where(o => (o.Type == 2 || o.Type == 3) && o.Status == 2)
                .ToListAsync();

            var orderForChef1DTOs = orders.Select(o => new OrderForChef1DTO
            {
                OrderId = o.OrderId,
                RecevingOrder = o.RecevingOrder,
                Status = o.Status,
                Type = o.Type,
                OrderDetails = _mapper.Map<IEnumerable<OrderDetailForChefDTO>>(o.OrderDetails)
            });

            return orderForChef1DTOs;
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
