using EHM_API.DTOs.OrderDetailDTO.Manager;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EHM_API.Services
{
    public interface IOrderDetailService
    {
        Task<IEnumerable<OrderDetailForChefDTO>> GetOrderDetailsAsync();
        Task<IEnumerable<OrderDetailForChefDTO>> GetOrderDetailSummaryAsync();
        Task UpdateDishesServedAsync(List<int> orderDetailIds);
        Task<IEnumerable<OrderDetailForChefDTO>> GetOrderDetailsByDishesServedAsync(int? dishesServed);
    }
}
