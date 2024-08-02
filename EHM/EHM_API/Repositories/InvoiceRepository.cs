using EHM_API.DTOs.CartDTO.OrderStaff;
using EHM_API.DTOs.OrderDTO.Manager;
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
				return null;
			}

			var order = invoice.Orders.FirstOrDefault();
			if (order == null)
			{
				return null;
			}

			var consigneeName = order.Address?.ConsigneeName ?? invoice.CustomerName;
			var guestPhone = order.GuestPhone ?? invoice.Phone;

			var invoiceDetailDTO = new InvoiceDetailDTO
			{
				InvoiceId = invoice.InvoiceId,
				PaymentAmount = invoice.PaymentAmount,
				ConsigneeName = consigneeName,
				GuestPhone = guestPhone,
				OrderDate = order.OrderDate,
				TotalAmount = order.TotalAmount,
				AmountReceived = invoice.AmountReceived,
				ReturnAmount = invoice.ReturnAmount,
				Taxcode = invoice.Taxcode,
                ItemInvoice = (order.OrderDetails ?? Enumerable.Empty<OrderDetail>()).Select(od => new ItemInvoiceDTO
                {
                    DishId = od.DishId ?? 0,
                    ItemName = od.Dish?.ItemName,
                    ComboId = od.ComboId ?? 0,
                    NameCombo = od.Combo?.NameCombo,
                    Price = (od.Dish?.Price ?? od.Combo?.Price) ?? 0,
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

			order.Status = 4;
			order.InvoiceId = invoice.InvoiceId;

            await _context.SaveChangesAsync();

            await _tableRepository.UpdateTableStatus(orderTable.TableId, 0);

            return invoice.InvoiceId;
        }


		public async Task<int> CreateInvoiceForOrder(int orderId, CreateInvoiceForOrder2DTO createInvoiceDto)
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
				PaymentStatus = 0,

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

			order.Status = 2;
			order.InvoiceId = invoice.InvoiceId;

			await _context.SaveChangesAsync();


			return invoice.InvoiceId;
		}


		public async Task UpdateInvoiceAndCreateGuestAsync(int invoiceId, UpdateInvoiceDTO dto)
		{
			var invoice = await _context.Invoices
				.Include(i => i.Account)
				.Include(i => i.Discount)
				.Include(i => i.InvoiceLogs)
				.Include(i => i.Orders)
				.FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);

			if (invoice == null)
			{
				throw new KeyNotFoundException($"Không tìm thấy hóa đơn với ID {invoiceId}.");
			}

			invoice.CustomerName = dto.CustomerName;
			invoice.Phone = dto.Phone;
			invoice.Address = dto.Address;

			// Tạo khách hàng nếu cần
			var guest = await _context.Guests.FirstOrDefaultAsync(g => g.GuestPhone == dto.Phone);

			if (guest == null)
			{
				guest = new Guest
				{
					GuestPhone = dto.Phone,
					// Bỏ qua Email nếu không cần
				};
				await _context.Guests.AddAsync(guest);
				await _context.SaveChangesAsync();
			}

			// Tạo địa chỉ nếu cần
			var address = await _context.Addresses.FirstOrDefaultAsync(a =>
				a.GuestAddress == dto.Address &&
				a.ConsigneeName == dto.CustomerName &&
				a.GuestPhone == dto.Phone);

			if (address == null)
			{
				address = new Address
				{
					GuestAddress = dto.Address,
					ConsigneeName = dto.CustomerName,
					GuestPhone = dto.Phone
				};
				await _context.Addresses.AddAsync(address);
				await _context.SaveChangesAsync();
			}

			await _context.SaveChangesAsync();
		}
        public async Task<Invoice> GetInvoiceByIdAsync(int invoiceId)
        {
            return await _context.Invoices
                .Include(i => i.Orders)
                .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);
        }

        public async Task UpdateInvoiceAsync(Invoice invoice)
        {
            _context.Invoices.Update(invoice);
            await _context.SaveChangesAsync();
        }
    }
}
