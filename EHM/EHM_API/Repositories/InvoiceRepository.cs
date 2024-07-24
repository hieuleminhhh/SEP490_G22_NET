using EHM_API.DTOs.CartDTO.OrderStaff;
using EHM_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace EHM_API.Repositories
{
	public class InvoiceRepository : IInvoiceRepository
	{
		private readonly EHMDBContext _context;
        private readonly ITableRepository _tableRepository;
        public InvoiceRepository(EHMDBContext context, ITableRepository tableRepository)
		{
			_context = context;
            _tableRepository = tableRepository;
        }
		public async Task<InvoiceDetailDTO> GetInvoiceDetailAsync(int invoiceId)
		{
			var invoice = await _context.Invoices
				.Include(i => i.Orders)
					.ThenInclude(o => o.OrderDetails)
					.ThenInclude(od => od.Dish)
				.Include(i => i.Orders)
					.ThenInclude(o => o.OrderDetails)
					.ThenInclude(od => od.Combo)
				.Include(i => i.Orders)
					.ThenInclude(o => o.Address)
				.FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);
			if (invoice == null)
			{
				throw new KeyNotFoundException($"Invoice with ID {invoiceId} not found.");
			}
			var order = invoice.Orders.FirstOrDefault();
			if (order == null)
			{
				throw new KeyNotFoundException($"Order for Invoice with ID {invoiceId} not found.");
			}
			var invoiceDetailDTO = new InvoiceDetailDTO
			{
				InvoiceId = invoice.InvoiceId,
				PaymentAmount = invoice.PaymentAmount,
				ConsigneeName = order.Address?.ConsigneeName,
				GuestPhone = order.GuestPhone,
				OrderDate = order.OrderDate,
				TotalAmount = order.TotalAmount,
				AmountReceived = invoice.AmountReceived,
				ReturnAmount = invoice.ReturnAmount,
				Taxcode = invoice.Taxcode,
				ItemInvoice = order.OrderDetails.Select(od => new ItemInvoiceDTO
				{
					DishId = od.DishId ?? 0,
					ItemName = od.Dish?.ItemName,
					ComboId = od.ComboId ?? 0,
					NameCombo = od.Combo?.NameCombo,
					Price = od.Dish?.Price ?? od.Combo?.Price ?? 0,
					UnitPrice = od.UnitPrice,
					Quantity = od.Quantity
				}).ToList()
			};
			return invoiceDetailDTO;
		}

		public async Task<int> CreateInvoiceForOrderAsync(int orderId, CreateInvoiceForOrderDTO createInvoiceDto)
        {
            var order = await _context.Orders
                .Include(o => o.Address)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy đơn hàng với OrderID {orderId}.");
            }

            // Retrieve the tableId associated with this order
            var orderTable = await _context.OrderTables
                .FirstOrDefaultAsync(ot => ot.OrderId == orderId);

            if (orderTable == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy bảng với OrderID {orderId}.");
            }

            var invoice = new Invoice
            {
                PaymentTime = createInvoiceDto.PaymentTime,
                PaymentAmount = createInvoiceDto.PaymentAmount,
                DiscountId = createInvoiceDto.DiscountId == 0 ? (int?)null : createInvoiceDto.DiscountId,
                Taxcode = createInvoiceDto.Taxcode,
                PaymentStatus = createInvoiceDto.PaymentStatus,

                CustomerName = order.Address?.ConsigneeName,
                Phone = order.Address?.GuestPhone,
                Address = order.Address?.GuestAddress,

                AmountReceived = createInvoiceDto.AmountReceived,
                ReturnAmount = createInvoiceDto.ReturnAmount,
                PaymentMethods = createInvoiceDto.PaymentMethods
            };

            await _context.Invoices.AddAsync(invoice);
            await _context.SaveChangesAsync();

            var invoiceLog = new InvoiceLog
            {
                Description = createInvoiceDto.Description,
                InvoiceId = invoice.InvoiceId
            };

            await _context.InvoiceLogs.AddAsync(invoiceLog);

			order.InvoiceId = invoice.InvoiceId;

            await _context.SaveChangesAsync();

            await _tableRepository.UpdateTableStatus(orderTable.TableId, 0);

            return invoice.InvoiceId;
        }





    }
}
