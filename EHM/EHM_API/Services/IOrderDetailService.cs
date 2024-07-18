using EHM_API.DTOs.OrderDetailDTO.Manager;
using EHM_API.DTOs.OrderDTO.Manager;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EHM_API.Services
{
    public interface IOrderDetailService
    {
        Task<bool> UpdateOrderDetailQuantityAsync(int orderDetailId, int quantity);
        Task<IEnumerable<OrderDetailForChefDTO>> GetOrderDetailsAsync();
        Task<IEnumerable<OrderDetailForChefDTO>> GetOrderDetails1Async();
        Task<IEnumerable<OrderDetailForChefDTO>> GetOrderDetailSummaryAsync();
        Task UpdateDishesServedAsync(int orderDetailId, int? dishesServed);
        Task<IEnumerable<OrderDetailForChefDTO>> GetOrderDetailsByDishesServedAsync(int? dishesServed);
    }
}
