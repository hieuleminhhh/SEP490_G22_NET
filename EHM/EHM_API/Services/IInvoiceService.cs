using EHM_API.DTOs.CartDTO.OrderStaff;

namespace EHM_API.Services
{
	public interface IInvoiceService
	{
		Task<InvoiceDetailDTO> GetInvoiceDetailAsync(int invoiceId);
		Task CreateInvoiceForOrderAsync(int orderId, CreateInvoiceForOrderDTO createInvoiceDto);
	}
}
