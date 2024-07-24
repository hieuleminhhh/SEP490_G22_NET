using EHM_API.DTOs.CartDTO.OrderStaff;
using EHM_API.Repositories;

namespace EHM_API.Services
{
	public class InvoiceService : IInvoiceService
	{
		private readonly IInvoiceRepository _invoiceRepository;

		public InvoiceService(IInvoiceRepository orderRepository)
		{
			_invoiceRepository = orderRepository;
		}

		public async Task<InvoiceDetailDTO> GetInvoiceDetailAsync(int invoiceId)
		{
			return await _invoiceRepository.GetInvoiceDetailAsync(invoiceId);
		}

        public async Task<int> CreateInvoiceForOrderAsync(int orderId, CreateInvoiceForOrderDTO createInvoiceDto)
        {
            return await _invoiceRepository.CreateInvoiceForOrderAsync(orderId, createInvoiceDto);
        }

    }
}
