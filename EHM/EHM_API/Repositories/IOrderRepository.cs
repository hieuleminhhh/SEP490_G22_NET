using EHM_API.DTOs.CartDTO.Guest;
using EHM_API.DTOs.HomeDTO;
using EHM_API.DTOs.OrderDTO.Cashier;
using EHM_API.DTOs.OrderDTO.Manager;
using EHM_API.DTOs.TableDTO.Manager;
using EHM_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IOrderRepository
{
    Task<IEnumerable<Order>> GetAllAsync();
    Task<Order> GetByIdAsync(int id);
    Task<Order> AddAsync(Order order);
    Task<Order> UpdateAsync(Order order);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<Order>> SearchAsync(string guestPhone);
    Task<Order> UpdateOrderStatusAsync(int orderId, int status);

    Task<IEnumerable<Order>> GetOrdersWithTablesAsync();
    Task<PagedResult<OrderDTO>> GetOrderAsync(string search, DateTime? dateFrom, DateTime? dateTo, int status, int page, int pageSize, string filterByDate, int type);
	Task<Order?> GetOrderByTableIdAsync(int tableId);

    Task UpdateOrderAsync(Order order);
    Task<Address?> GetOrCreateAddress2(CheckoutDTO checkoutDTO);
    Task<Order> UpdateOrderForTable(int tableId, UpdateTableAndGetOrderDTO dto);
    Task<Order?> UpdateOrderDetailsByOrderId(int orderId, UpdateTableAndGetOrderDTO dto);
	Task<Order> CreateOrderForTable(int tableId, CreateOrderForTableDTO dto);

    Task UpdateOrderStatusForTableAsync(int tableId, int orderId, UpdateOrderStatusForTableDTO dto);
    Task CancelOrderAsync(int tableId, int orderId, CancelOrderDTO dto);

	Task<IEnumerable<OrderDetail>> GetOrderDetailsByOrderIdAsync(int orderId);

    Task<IEnumerable<Order>> GetOrdersByTableIdAsync(int tableId);
    Task<int> CountOrderByDiscountIdAsync(int discountId);
    Task<IEnumerable<Order>> GetOrderDetailsForStaffType1Async();
    Task<Order?> UpdateCancelationReasonAsync(int orderId, string? reason, string? cancelBy);

    Task<Order?> GetOrderById(int orderId);
	Task<IEnumerable<OrderTable>> GetOrderTablesByOrderIdAsync(int orderId);
    Task<IEnumerable<Order>> GetOrdersByStatusAndAccountIdAsync(int status, int accountId);
    Task<List<OrderStatisticsDTO>> GetOrderStatisticsByCollectedByAsync(DateTime? startDate, DateTime? endDate, int? collectedById);
    Task<Dictionary<int, int>> GetSalesByCategoryAsync(DateTime? startDate, DateTime? endDate);
    Task<List<Order?>> GetOrderByIdAsync(List<int> orderIds);

    Task<Order> CreateOrderForReservation(int tableId, CreateOrderForReservaionDTO dto);
    Task<Order?> GetOrderByIdAsync(int orderId);
    Task UpdateOrderForTotalAsync(Order order);
    Task<Order?> GetOrderById1Async(int orderId);
    Task<Order?> GetOrderByStaffIdAsync(int staffId);
    Task<List<Order>> GetOrdersByStatusAsync(int status);
    Task SaveAsync();
    Task UpdateOrderShipTimeAsync(Order order);
    Task<bool> UpdateAcceptByAsync(UpdateAcceptByDTO dto);
}

