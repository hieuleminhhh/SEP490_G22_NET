using EHM_API.DTOs.CartDTO.OrderStaff;
using EHM_API.DTOs.InvoiceDTO;
using EHM_API.DTOs.OrderDTO.Manager;
using EHM_API.Models;
using Microsoft.EntityFrameworkCore;

namespace EHM_API.Repositories
{
	public interface IInvoiceRepository
	{
		Task<InvoiceDetailDTO> GetInvoiceDetailAsync(int invoiceId);
		Task<int> CreateInvoiceForOrderAsync(int orderId, CreateInvoiceForOrderDTO createInvoiceDto);
		Task<int> CreateInvoiceForOrder(int orderId, CreateInvoiceForOrder2DTO createInvoiceDto);
		Task UpdateInvoiceAndCreateGuestAsync(int invoiceId, UpdateInvoiceDTO dto);

        Task<Invoice> GetInvoiceByIdAsync(int invoiceId);
        Task UpdateInvoiceAsync(Invoice invoice);
		Task<Invoice> GetInvoiceByOrderIdAsync(int orderId);
		Task CreateInvoiceAsync(Invoice invoice);

		Task CreateInvoiceLog(InvoiceLog invoiceLog);
		Task UpdateOrderAndInvoiceAsync(int orderId, InvoiceOfSitting dto);

		Task<IEnumerable<Order>> GetAllOrdersWithInvoicesAsync();
		Task<IEnumerable<Order>> GetOrdersWithInvoicesByStatusAndDepositAsync(int status, decimal minDeposit);

		Task<List<Order>> GetOrdersUnpaidForShipAsync();

		Task<Order> GetOrderByIdAsync(int orderId);
    }
}
