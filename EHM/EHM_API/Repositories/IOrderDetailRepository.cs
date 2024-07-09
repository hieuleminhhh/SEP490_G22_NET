using EHM_API.DTOs.OrderDetailDTO.Manager;
using EHM_API.DTOs.OrderDTO.Manager;
using EHM_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EHM_API.Repositories
{
    public interface IOrderDetailRepository
    {
        Task<IEnumerable<OrderForChefDTO>> GetOrderDetailsAsync();
        Task<IEnumerable<OrderForChef1DTO>> GetOrderDetails1Async();
        Task<IEnumerable<OrderDetailForChefDTO>> GetOrderDetailSummaryAsync();
        Task UpdateDishesServedAsync(List<int> orderDetailIds);
        Task<IEnumerable<OrderDetail>> GetOrderDetailsByDishesServedAsync(int? dishesServed);

    }
}
