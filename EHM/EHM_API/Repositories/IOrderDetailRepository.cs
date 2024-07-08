using EHM_API.DTOs.OrderDetailDTO.Manager;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EHM_API.Repositories
{
    public interface IOrderDetailRepository
    {
        Task<IEnumerable<OrderDetailForChefDTO>> GetOrderDetailsAsync();
        Task<IEnumerable<OrderDetailForChefDTO>> GetOrderDetailSummaryAsync();
        Task UpdateDishesServedAsync(List<int> orderDetailIds);

    }
}
