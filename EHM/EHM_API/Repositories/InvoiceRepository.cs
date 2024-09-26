using EHM_API.DTOs.CartDTO.Guest;
using EHM_API.DTOs.CartDTO.OrderStaff;
using EHM_API.DTOs.InvoiceDTO;
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
                .Include(i => i.Orders)
                    .ThenInclude(o => o.Discount)
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
            var guestAddress = order.Address?.GuestAddress ?? invoice.Address;

            var invoiceDetailDTO = new InvoiceDetailDTO
            {
                InvoiceId = invoice.InvoiceId,
                PaymentAmount = invoice.PaymentAmount,
                PaymentTime = invoice.PaymentTime,
                PaymentStatus = invoice.PaymentStatus,
                ConsigneeName = consigneeName,
                GuestPhone = guestPhone,
                Address = guestAddress,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                AmountReceived = invoice.AmountReceived,
                ReturnAmount = invoice.ReturnAmount,
                Taxcode = invoice.Taxcode,
                DiscountName = order.Discount?.DiscountName,
                DiscountPercent = order.Discount?.DiscountPercent,
                Note = order.Discount?.Note,
                TotalMoney = order.Discount?.TotalMoney,
                QuantityLimit = order.Discount?.QuantityLimit,
                Deposits = order.Deposits ?? 0m, // Sửa lỗi nullable
                ItemInvoice = (order.OrderDetails ?? Enumerable.Empty<OrderDetail>()).Select(od => new ItemInvoiceDTO
                {
                    DishId = od.DishId ?? 0,  // Sửa lỗi nullable
                    ItemName = od.Dish?.ItemName,
                    ComboId = od.ComboId ?? 0,  // Sửa lỗi nullable
                    NameCombo = od.Combo?.NameCombo,
                    Price = (od.Dish?.Price ?? od.Combo?.Price) ?? 0,
                    UnitPrice = od.UnitPrice ?? 0,  // Sửa lỗi nullable
                    Quantity = od.Quantity ?? 0  // Sửa lỗi nullable
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
                PaymentTime = DateTime.Now,
                PaymentAmount = createInvoiceDto.PaymentAmount,
                Taxcode = createInvoiceDto.Taxcode,
                PaymentStatus = createInvoiceDto.PaymentStatus,

                CustomerName = order.Address?.ConsigneeName,
                Phone = order.Address?.GuestPhone,
                Address = order.Address?.GuestAddress,

				AccountId = createInvoiceDto.AccountId != 0 ? createInvoiceDto.AccountId : (int?)null,
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
			order.RecevingOrder = DateTime.Now;

            await _context.SaveChangesAsync();

			await _tableRepository.UpdateTableStatusByOrderId(orderId, 0);

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
				Taxcode = createInvoiceDto.Taxcode,
				PaymentStatus = createInvoiceDto.PaymentStatus,

				CustomerName = order.Address?.ConsigneeName,
				Phone = order.Address?.GuestPhone,
				Address = order.Address?.GuestAddress,

				AccountId = createInvoiceDto.AccountId != 0 ? createInvoiceDto.AccountId : (int?)null,
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

		public async Task<Invoice> GetInvoiceByOrderIdAsync(int orderId)
		{
			return await _context.Orders
                .Include(d => d.Discount)
				.Where(o => o.OrderId == orderId)
				.Select(o => o.Invoice)
				.FirstOrDefaultAsync();
		}

		public async Task CreateInvoiceAsync(Invoice invoice)
		{
			_context.Invoices.Add(invoice);
			await _context.SaveChangesAsync();
		}

		public async Task CreateInvoiceLog(InvoiceLog invoiceLog)
		{
			_context.InvoiceLogs.Add(invoiceLog);
			await _context.SaveChangesAsync();
		}


		public async Task<Guest> GetOrCreateGuest(InvoiceOfSitting dto)
		{

			if (string.IsNullOrWhiteSpace(dto.Phone))
			{
				return null;
			}

			var guest = await _context.Guests
				.FirstOrDefaultAsync(g => g.GuestPhone == dto.Phone);

			if (guest != null)
			{
			
			}
			else
			{
			
				guest = new Guest
				{
					GuestPhone = dto.Phone
				};
				await _context.Guests.AddAsync(guest);
			}

			await _context.SaveChangesAsync();
			return guest;
		}

		public async Task<Address> GetOrCreateAddress(InvoiceOfSitting dto)
		{
			if (string.IsNullOrWhiteSpace(dto.Address) ||
				string.IsNullOrWhiteSpace(dto.CustomerName) ||
				string.IsNullOrWhiteSpace(dto.Phone))
			{
				return null;
			}

			var address = await _context.Addresses
				.FirstOrDefaultAsync(a =>
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

			return address;
		}

		public async Task UpdateOrderAndInvoiceAsync(int orderId, InvoiceOfSitting dto)
		{
			var order = await _context.Orders
				.Include(o => o.Invoice)
				.Include(o => o.OrderTables)
					.ThenInclude(ot => ot.Table)
				.FirstOrDefaultAsync(o => o.OrderId == orderId);

			if (order == null)
			{
				throw new KeyNotFoundException($"Không tìm thấy đơn hàng {orderId}.");
			}

			var guest = await GetOrCreateGuest(dto);
			var address = await GetOrCreateAddress(dto);

			order.Status = dto.Status;
			order.RecevingOrder = DateTime.Now;

			if (order.Invoice != null)
			{
				order.Invoice.PaymentTime = DateTime.Now;
				order.Invoice.PaymentAmount = dto.PaymentAmount;
				order.Invoice.Taxcode = dto.Taxcode;
				order.Invoice.PaymentStatus = dto.PaymentStatus;

				order.Invoice.CustomerName = address?.GuestAddress ?? dto.CustomerName;
				order.Invoice.Phone = guest?.GuestPhone ?? dto.Phone;
				order.Invoice.Address = address?.GuestAddress ?? dto.Address;

				order.Invoice.AmountReceived = dto.AmountReceived;
				order.Invoice.ReturnAmount = dto.ReturnAmount;
				order.Invoice.PaymentMethods = dto.PaymentMethods;

			

				if (!string.IsNullOrEmpty(dto.Description))
				{
					var invoiceLog = new InvoiceLog
					{
						InvoiceId = order.Invoice.InvoiceId,
						Description = dto.Description
					};
					_context.InvoiceLogs.Add(invoiceLog);
				}
			}

			foreach (var orderTable in order.OrderTables)
			{
				orderTable.Table.Status = dto.TableStatus ?? 0;
			}

			await _context.SaveChangesAsync();
		}

		public async Task<IEnumerable<Order>> GetAllOrdersWithInvoicesAsync()
		{
			return await _context.Orders
				.Include(o => o.Invoice)
				.Where(o => o.InvoiceId.HasValue)
				.ToListAsync();
		}

        public async Task<IEnumerable<Order>> GetOrdersWithInvoicesByStatusAndDepositAsync(int status, decimal minDeposit)
        {
            return await _context.Orders
                .Include(o => o.Address)
                .Include(o => o.Staff)
                .Include(o => o.Collected)
                .Include(o => o.Invoice)
                .Include(o => o.Reservations)
                .Where(o =>
                    (o.Status == status || o.Status == 8)
                    && o.Deposits > minDeposit
                    && (o.InvoiceId == null || (o.InvoiceId.HasValue && (o.Invoice.PaymentStatus == 2 || o.Invoice.PaymentStatus == 1)))
                   
                    && !o.Reservations.Any(r => r.Status == 5 && r.ReservationTime.Value.Date < DateTime.Now.Date && o.Deposits > 0)
                )
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }




        public async Task<List<Order>> GetOrdersUnpaidForShipAsync()
        {
            return await _context.Orders
                .Include(o => o.Account)
                .Include(o => o.Staff)
                .Include(o => o.Collected)
                .Include(o => o.Invoice)
                .Include(o => o.Discount)
                .Where(o => o.Type == 2 && o.Status == 4 && o.Deposits == 0)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }




        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.Invoice)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }
        public async Task<decimal> GetTotalPaymentAmountAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Invoices
                .Where(i => i.PaymentStatus == 1
                            && i.PaymentMethods == 0
                            && i.Orders.Any(o => o.Status == 4) 
                            && i.PaymentTime >= startDate
                            && i.PaymentTime <= endDate)
                .SumAsync(i => i.PaymentAmount ?? 0); 
        }
    }
}
