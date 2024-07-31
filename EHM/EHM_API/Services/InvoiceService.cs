using AutoMapper;
using EHM_API.DTOs.CartDTO.OrderStaff;
using EHM_API.Repositories;

namespace EHM_API.Services
{
	public class InvoiceService : IInvoiceService
	{
		private readonly IInvoiceRepository _invoiceRepository;
		private readonly IMapper _mapper;

		public InvoiceService(IInvoiceRepository orderRepository, IMapper mapper)
		{
			_invoiceRepository = orderRepository;
			_mapper = mapper;
		}

		public async Task<InvoiceDetailDTO> GetInvoiceDetailAsync(int invoiceId)
		{
			var invoiceDetail = await _invoiceRepository.GetInvoiceDetailAsync(invoiceId);
			if (invoiceDetail == null)
			{
				return null;
			}

			invoiceDetail.ItemInvoice = CombineInvoiceItems(invoiceDetail.ItemInvoice);
			return invoiceDetail;
		}

		private IEnumerable<ItemInvoiceDTO> CombineInvoiceItems(IEnumerable<ItemInvoiceDTO> items)
		{
			return items
				.GroupBy(item => new { item.DishId, item.ComboId })
				.Select(g =>
				{
					var first = g.First();
					return new ItemInvoiceDTO
					{
						DishId = first.DishId,
						ItemName = first.ItemName,
						ComboId = first.ComboId,
						NameCombo = first.NameCombo,
						Price = first.Price,
						UnitPrice = g.Sum(item => item.UnitPrice),
						Quantity = g.Sum(item => item.Quantity)
					};
				})
				.ToList();
		}
		public async Task<int> CreateInvoiceForOrderAsync(int orderId, CreateInvoiceForOrderDTO createInvoiceDto)
        {
            return await _invoiceRepository.CreateInvoiceForOrderAsync(orderId, createInvoiceDto);
        }

		public async Task UpdateInvoiceAndCreateGuestAsync(int invoiceId, UpdateInvoiceDTO dto)
		{
			await _invoiceRepository.UpdateInvoiceAndCreateGuestAsync(invoiceId, dto);
		}

	}
}
