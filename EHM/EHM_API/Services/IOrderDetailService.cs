using EHM_API.DTOs.CartDTO.OrderStaff;
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
        Task<IEnumerable<OrderDetailForChef1DTO>> GetOrderDetails1Async();
        Task<IEnumerable<OrderDetailForChefDTO>> GetOrderDetailSummaryAsync();
        Task UpdateDishesServedAsync(int orderDetailId, int? dishesServed);
        Task<IEnumerable<OrderDetailForChefDTO>> GetOrderDetailsByDishesServedAsync(int? dishesServed);
        Task<IEnumerable<OrderDetailForStaff>> SearchByDishOrComboNameAsync(string keyword);
        Task<GetRemainingItemsResponseDTO> GetRemainingItemsAsync(List<int> comboIds, List<int> dishIds);
    }
}
