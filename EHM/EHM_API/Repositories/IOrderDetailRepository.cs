using EHM_API.DTOs.OrderDetailDTO.Manager;
using EHM_API.DTOs.OrderDTO.Manager;
using EHM_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EHM_API.Repositories
{
    public interface IOrderDetailRepository
    {
        Task<IEnumerable<OrderDetailForChefDTO>> GetOrderDetailsAsync();
        Task<IEnumerable<OrderDetailForChefDTO>> GetOrderDetailSummaryAsync();
        Task UpdateDishesServedAsync(int orderDetailId, int? dishesServed);
        Task<IEnumerable<OrderDetail>> GetOrderDetailsByDishesServedAsync(int? dishesServed);

    }
}
