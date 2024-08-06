using EHM_API.DTOs.CartDTO.OrderStaff;
using EHM_API.DTOs.OrderDTO.Manager;
using EHM_API.Models;

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
	}
}
