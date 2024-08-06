using EHM_API.DTOs.CartDTO.OrderStaff;
using EHM_API.DTOs.OrderDTO.Manager;

namespace EHM_API.Services
{
	public interface IInvoiceService
	{
		Task<InvoiceDetailDTO> GetInvoiceDetailAsync(int invoiceId);
		Task<int> CreateInvoiceForOrderAsync(int orderId, CreateInvoiceForOrderDTO createInvoiceDto);
		Task<int> CreateInvoiceForOrder(int orderId, CreateInvoiceForOrder2DTO createInvoiceDto);

		Task UpdateInvoiceAndOrderAsync(int orderId, UpdateInvoiceSuccessPaymentDTO dto);
		Task UpdateOrderStatusAsync(int orderId, UpdateStatusOrderDTO dto);
		Task UpdateInvoiceAndCreateGuestAsync(int invoiceId, UpdateInvoiceDTO dto);
		Task<GetInvoiceByOrderDTO> GetInvoiceByOrderIdAsync(int orderId);
	}
}
