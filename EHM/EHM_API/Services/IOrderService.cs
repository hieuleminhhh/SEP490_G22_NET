using EHM_API.DTOs.AccountDTO;
using EHM_API.DTOs.CartDTO.Guest;
using EHM_API.DTOs.HomeDTO;
using EHM_API.DTOs.OrderDetailDTO.Manager;
using EHM_API.DTOs.OrderDTO.Cashier;
using EHM_API.DTOs.OrderDTO.Cashier.EHM_API.DTOs.OrderDTO;
using EHM_API.DTOs.OrderDTO.Guest;
using EHM_API.DTOs.OrderDTO.Manager;
using EHM_API.DTOs.TableDTO;
using EHM_API.DTOs.TableDTO.Manager;
using EHM_API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EHM_API.Services
{
    public interface IOrderService
    {
        Task<OrderDTOAll> CreateOrderAsync(CreateOrderDTO createOrderDto);
        Task<IEnumerable<OrderDTOAll>> GetAllOrdersAsync();

		Task<IEnumerable<SearchPhoneOrderDTO>> GetAllOrdersToSearchAsync();
		Task<OrderDTOAll> GetOrderByIdAsync(int id);
        Task<IEnumerable<SearchPhoneOrderDTO>> SearchOrdersAsync(string guestPhone = null);
        Task<OrderDTOAll> UpdateOrderAsync(int id, UpdateOrderDTO updateOrderDto);
        Task<bool> DeleteOrderAsync(int id);

		Task<bool> CancelOrderAsync(int orderId);
        Task<PagedResult<OrderDTO>> GetOrderAsync(string search, DateTime? dateFrom, DateTime? dateTo, int status, int page, int pageSize, string filterByDate, int type);
        Task<Order> UpdateOrderStatusAsync(int comboId, int status);

		Task<IEnumerable<ListTableOrderDTO>> GetOrdersWithTablesAsync();
		Task<FindTableAndGetOrderDTO?> GetOrderByTableIdAsync(int tableId);

        Task<Order?> UpdateOrderDetailsAsync(int tableId, UpdateTableAndGetOrderDTO dto);

		Task<Order?> UpdateOrderDetailsByOrderIdAsync(int orderId, UpdateTableAndGetOrderDTO dto);
		Task<Order> CreateOrderForTable(int tableId, CreateOrderForTableDTO dto);

		Task UpdateOrderStatusForTableAsync(int tableId, int orderId, UpdateOrderStatusForTableDTO dto);
		Task CancelOrderForTableAsync(int tableId, int orderId, CancelOrderDTO dto);
		Task<IEnumerable<GetOrderDetailDTO>> GetOrderDetailsByOrderIdAsync(int orderId);
		Task<IEnumerable<GetDishOrderDetailDTO>> GetOrderDetailsByOrderId(int orderId);
		Task UpdateOrderAndTablesStatusAsyncByTableId(int tableId, CancelOrderTableDTO dto);
		Task<int> UpdateStatusAndCreateInvoiceAsync(int orderId, UpdateStatusAndCInvoiceD dto);
        Task<IEnumerable<OrderDetailForStaffType1>> GetOrderDetailsForStaffType1Async();
		Task UpdateAmountReceivingAsync(int orderId, UpdateAmountReceiving dto);
        Task<CancelationReasonDTO?> UpdateCancelationReasonAsync(int orderId, CancelationReasonDTO? cancelationReasonDTO);
		Task AcceptOrderAsync(int orderId, AcceptOrderDTO acceptOrderDto);
        Task<OrderAccountDTO?> UpdateAccountIdAsync(int orderId, UpdateOrderAccountDTO updateOrderAccountDTO);
        Task<IEnumerable<OrderDetailForStaffType1>> GetOrdersByStatusAndAccountIdAsync(int status, int staffId);
        Task<Order> UpdateForOrderStatusAsync(int orderId, int status);
        Task<List<OrderStatisticsDTO>> GetOrderStatisticsAsync(DateTime? startDate, DateTime? endDate, int? collectedById);
        Task<IEnumerable<CategorySalesDTO>> GetSalesByCategoryAsync(DateTime? startDate, DateTime? endDate);
        Task<List<ExportOrderDTO?>> GetOrderDetailsByIdsAsync(List<int> orderIds);

        Task<Order> CreateOrderForReservation(int tableId, CreateOrderForReservaionDTO dto);
        Task<UpdateTotalAmountDTO?> UpdateTotalAmountAsync(int orderId);
        Task<OrderEmailDTO> GetEmailByOrderIdAsync(int orderId);
        Task<bool> UpdateStaffByOrderIdAsync(UpdateStaffDTO updateStaffDTO);
        Task<List<OrderDetailWithStaffDTO>> GetOrdersWithStatus8Async();
        Task<bool> UpdateAcceptByAsync(UpdateAcceptByDTO dto);
        Task<List<CashierReportDTO>> GetCashierReportAsync(DateTime? startDate, DateTime? endDate, int? collectedById);
    }
}
