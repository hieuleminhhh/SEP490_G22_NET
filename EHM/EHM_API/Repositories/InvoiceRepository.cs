using EHM_API.DTOs.CartDTO.OrderStaff;
using EHM_API.Models;
using Microsoft.EntityFrameworkCore;

namespace EHM_API.Repositories
{
	public class InvoiceRepository : IInvoiceRepository
	{
		private readonly EHMDBContext _context;

		public InvoiceRepository(EHMDBContext context)
		{
			_context = context;
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
				return null;
			}

			var order = invoice.Orders.FirstOrDefault();
			if (order == null)
			{
				return null;
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
					Price = od.Dish.Price ?? od.Combo.Price,
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

            return invoice.InvoiceId;
        }




    }
}
