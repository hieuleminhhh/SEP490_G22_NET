using EHM_API.DTOs.OrderDetailDTO.Manager;
using EHM_API.DTOs.OrderDTO.Manager;
using EHM_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EHM_API.Repositories
{
    public interface IOrderDetailRepository
    {
        Task<OrderDetail> GetOrderDetailByIdAsync(int orderDetailId);
        Task<bool> UpdateOrderDetailAsync(OrderDetail orderDetail);
        Task<IEnumerable<OrderDetailForChefDTO>> GetOrderDetailsAsync();
        Task<IEnumerable<OrderDetailForChef1DTO>> GetOrderDetails1Async();
        Task<IEnumerable<OrderDetailForChefDTO>> GetOrderDetailSummaryAsync();
        Task UpdateDishesServedAsync(int orderDetailId, int? dishesServed);
        Task<IEnumerable<OrderDetail>> GetOrderDetailsByDishesServedAsync(int? dishesServed);
        Task<IEnumerable<OrderDetail>> SearchByDishOrComboNameAsync(string keyword);
        Task<List<OrderDetail>> GetRelevantOrderDetailsAsync(DateTime today);
    }
}
