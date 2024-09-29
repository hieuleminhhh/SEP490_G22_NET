using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EHM_API.DTOs.OrderDetailDTO.Manager;
using EHM_API.DTOs.OrderDTO.Manager;
using EHM_API.Models;
using Microsoft.EntityFrameworkCore;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

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
        public async Task<OrderDetail> GetOrderDetailByIdAsync(int orderDetailId)
        {
            return await _context.OrderDetails.FindAsync(orderDetailId);
        }

        public async Task<bool> UpdateOrderDetailAsync(OrderDetail orderDetail)
        {
            _context.OrderDetails.Update(orderDetail);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<IEnumerable<OrderDetailForChefDTO>> GetOrderDetailsAsync()
        {
            var now = DateTime.Now;
            var today = now.Date;

            var orderDetails = await _context.OrderDetails
                .Include(od => od.Dish)
                .Include(od => od.Combo)
                    .ThenInclude(c => c.ComboDetails)
                    .ThenInclude(cd => cd.Dish)
                .Include(od => od.Order)
                .Where(od =>
                    (
                      // Type = 1: Mang về
                      (od.Order.Type == 1 && od.Order.OrderDate.HasValue
                      && (od.Order.Status == 6 || od.Order.Status == 2)
                    && ((od.Order.RecevingOrder == null && od.Order.OrderDate.Value.Date ==today)
                    ||(od.Order.RecevingOrder.Value.Date == od.Order.OrderDate.Value.Date)))
                        // Type = 2: Online
                        || (od.Order.Type == 2
                            && od.Order.RecevingOrder.HasValue
                            && (od.Order.Status == 6 || od.Order.Status == 2)
                            && od.OrderTime.HasValue 
                            && (od.OrderTime.Value.Date == od.Order.RecevingOrder.Value.Date
                            || od.Order.RecevingOrder.Value.Date == od.Order.OrderDate.Value.Date))
                        // Type = 3: Đặt bàn
                        || (od.Order.Type == 3 && od.Order.Status == 3 && od.Order.RecevingOrder.HasValue
                        && od.Order.OrderDate.HasValue
                        && (od.Order.OrderDate.Value.Date == od.Order.RecevingOrder.Value.Date)
                        && (od.Order.OrderDate.Value.Date == od.OrderTime.Value.Date
                        || od.Order.OrderDate.Value.TimeOfDay != od.OrderTime.Value.TimeOfDay))
                        // Type = 4: Tại chỗ
                        || (od.Order.Type == 4 && od.OrderTime.HasValue
                       && (od.Order.Status == 3)
                            && od.OrderTime.Value.Date == od.Order.OrderDate.Value.Date)
                    )
                    // Điều kiện trạng thái
                    && od.Order.OrderDate.Value.Date == today
                   
                    && od.DishesServed < od.Quantity)

               .OrderBy(od =>
            od.Order.Type == 3 && od.Order.RecevingOrder.HasValue && od.OrderTime.HasValue
                ? (od.Order.RecevingOrder.Value > od.OrderTime.Value.AddHours(1)
                    ? od.Order.RecevingOrder.Value
                    : od.OrderTime.Value)
                : (od.Order.RecevingOrder.HasValue
                    ? od.Order.RecevingOrder.Value.AddHours(-1)
                    : (od.OrderTime.HasValue ? od.OrderTime.Value : od.Order.OrderDate.Value)))
        .ToListAsync();

            var orderDetailDTOs = _mapper.Map<IEnumerable<OrderDetailForChefDTO>>(orderDetails);
            return orderDetailDTOs;
        }



        public async Task<IEnumerable<OrderDetailForChef1DTO>> GetOrderDetails1Async()
        {
            var today = DateTime.Today;
            var now = DateTime.Now;

            var orderDetails = await _context.OrderDetails
                .Include(od => od.Dish)
                .Include(od => od.Combo)
                    .ThenInclude(c => c.ComboDetails)
                    .ThenInclude(cd => cd.Dish)
                .Include(od => od.Order)
                .Where(od => (
                        // Type = 1: Mang về
                        (od.Order.Type == 1
                            && od.Order.RecevingOrder.HasValue
                             && (od.Order.Status == 6 || od.Order.Status == 2)
                            && od.Order.OrderDate.HasValue
                            && od.Order.OrderDate.Value.Date != od.Order.RecevingOrder.Value.Date)
                        // Type = 2: Online
                        || (od.Order.Type == 2
                            && od.Order.RecevingOrder.HasValue
                            && (od.Order.Status == 6 || od.Order.Status == 2)
                            && od.Order.OrderDate.HasValue
                            && od.Order.OrderDate.Value.Date != od.Order.RecevingOrder.Value.Date)
                        // Type = 3: Đặt bàn
                        || (od.Order.Type == 3
                            && od.Order.RecevingOrder.HasValue
                            && (od.Order.Status == 2 || od.Order.Status == 3)
                            && od.OrderTime.HasValue
                            && od.OrderTime.Value.Date < od.Order.RecevingOrder.Value.Date)

                    )
                    // Điều kiện trạng thái
                    && od.DishesServed < od.Quantity
                    && od.OrderTime.HasValue
                    && od.Order.RecevingOrder.Value.Date >= today)
                // Sắp xếp theo thời gian RecevingOrder
                .OrderBy(od => od.Order.RecevingOrder)
                .ThenBy(od => od.OrderTime)
                .ToListAsync();

            var orderDetailDTO1s = _mapper.Map<IEnumerable<OrderDetailForChef1DTO>>(orderDetails);
            return orderDetailDTO1s;
        }




        public async Task<IEnumerable<OrderDetailForChefDTO>> GetOrderDetailSummaryAsync()
        {
            var today = DateTime.Today;
            var orderDetails = await _context.OrderDetails
                .Include(od => od.Dish)
                .Include(od => od.Combo)
                    .ThenInclude(c => c.ComboDetails)
                    .ThenInclude(cd => cd.Dish)
                    .Include(od => od.Order)
                    .Where(od => (od.Order.Type == 1 || od.Order.Type == 4) && od.Order.Status == 2 && od.OrderTime.HasValue && od.OrderTime.Value.Date == today)
                .ToListAsync();

            var result = orderDetails
                .GroupBy(od => new { od.DishId, od.ComboId })
                .Select(g => new OrderDetailForChefDTO
                {
                    ItemName = g.First().Dish?.ItemName,
                    Quantity = g.Sum(od => od.Quantity),
                    OrderTime = g.First().OrderTime,
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
                        QuantityDish = cd.QuantityDish
                    }).ToList(),
                    Note = g.First().Combo.Note,
                    OrderTime = g.First().OrderTime
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
        public async Task<IEnumerable<OrderDetail>> SearchByDishOrComboNameAsync(string keyword)
        {
            return await _context.OrderDetails
                .Include(od => od.Order)
                .ThenInclude(od => od.OrderTables)
                 .ThenInclude(ot => ot.Table)
                .Include(od => od.Dish)
                .Include(od => od.Combo)  
                .Where(od => (od.Dish.ItemName.Contains(keyword) || od.Combo.NameCombo.Contains(keyword))
                && od.Order.Status == 3 && od.OrderTime.HasValue 
                && od.OrderTime.Value.Date == DateTime.Today
                && od.Quantity > od.DishesServed)
                .ToListAsync();
        }
        public async Task<List<OrderDetail>> GetRelevantOrderDetailsAsync(DateTime today)
        {
            return await _context.OrderDetails
                .Include(od => od.Order)
                .Where(od =>
                    (od.Order.RecevingOrder.HasValue && od.Order.RecevingOrder.Value.Date == today)
                    || (!od.Order.RecevingOrder.HasValue && od.Order.OrderDate.HasValue && od.Order.OrderDate.Value.Date == today)
                )
                .Where(od => od.Order.Status == 2 || od.Order.Status == 3 || od.Order.Status == 4 || od.Order.Status == 6 || od.Order.Status == 7)
                .ToListAsync();
        }
    }
}
