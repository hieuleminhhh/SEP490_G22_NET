using EHM_API.DTOs.CartDTO.OrderStaff;

namespace EHM_API.Repositories
{
	public interface IInvoiceRepository
	{
		Task<InvoiceDetailDTO> GetInvoiceDetailAsync(int invoiceId);
		Task<int> CreateInvoiceForOrderAsync(int orderId, CreateInvoiceForOrderDTO createInvoiceDto);

		Task UpdateInvoiceAndCreateGuestAsync(int invoiceId, UpdateInvoiceDTO dto);


	}
}
