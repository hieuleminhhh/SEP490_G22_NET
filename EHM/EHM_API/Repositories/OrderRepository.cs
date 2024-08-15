using EHM_API.DTOs.CartDTO.Guest;
using EHM_API.DTOs.ComboDTO.Guest;
using EHM_API.DTOs.DishDTO.Manager;
using EHM_API.DTOs.HomeDTO;
using EHM_API.DTOs.OrderDTO.Manager;
using EHM_API.DTOs.TableDTO;
using EHM_API.DTOs.TableDTO.Manager;
using EHM_API.Models;
using EHM_API.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

public class OrderRepository : IOrderRepository
{
	private readonly EHMDBContext _context;
	private readonly IComboRepository _comboRepository;
	private readonly ITableRepository _tableRepository;
	private readonly IDiscountRepository _discountRepository;
	private readonly IDishRepository _dishRepository;
	private readonly ICartRepository _cartRepository;

	public OrderRepository(EHMDBContext context, ICartRepository cartRepository, IComboRepository comboRepository, IDishRepository dishRepository, ITableRepository tableRepository, IDiscountRepository discountRepository)
	{
		_context = context;
		_cartRepository = cartRepository;
		_comboRepository = comboRepository;
		_dishRepository = dishRepository;
		_tableRepository = tableRepository;
		_discountRepository = discountRepository;
	}

	public async Task<IEnumerable<Order>> GetAllAsync()
	{
		return await _context.Orders.Include(o => o.Account)
									.Include(o => o.Address).
									 ToListAsync();
	}


	public async Task<Order> GetByIdAsync(int id)
	{
		return await _context.Orders
			.Include(o => o.Account)
			.Include(d => d.Discount)
			.Include(o => o.Address)
			.Include(o => o.Invoice)
			.Include(o => o.OrderDetails)
				.ThenInclude(od => od.Combo)
			.Include(o => o.OrderDetails)
				.ThenInclude(od => od.Dish)
				.ThenInclude(d => d.Discount)
			.Include(o => o.OrderTables)
				.ThenInclude(ot => ot.Table)
			.FirstOrDefaultAsync(o => o.OrderId == id);
	}




	public async Task<Order> AddAsync(Order order)
	{
		_context.Orders.Add(order);
		await _context.SaveChangesAsync();
		return order;
	}
	public async Task<Account> GetAccountByUsernameAsync(string username)
	{
		return await _context.Accounts.FirstOrDefaultAsync(a => a.Username == username);
	}

	public async Task<IEnumerable<Order>> SearchAsync(string guestPhone)
	{
		return await _context.Orders
								.Include(o => o.Account)
								.Include(o => o.Address)
								.Include(o => o.OrderDetails)
								.ThenInclude(od => od.Combo)
								.Include(o => o.OrderDetails)
								.ThenInclude(od => od.Dish)
								.ThenInclude(d => d.Discount)
								.Include(d => d.Discount)
								.Where(o => o.GuestPhone == guestPhone)
								 .OrderByDescending(o => o.OrderDate)
								.ToListAsync();
	}



	public async Task<Order> UpdateAsync(Order order)
	{
		_context.Entry(order).State = EntityState.Modified;
		await _context.SaveChangesAsync();
		return order;
	}

	public async Task<bool> DeleteAsync(int id)
	{
		var order = await _context.Orders.FindAsync(id);
		if (order == null)
		{
			return false;
		}

		_context.Orders.Remove(order);
		await _context.SaveChangesAsync();
		return true;
	}
	public async Task<PagedResult<OrderDTO>> GetOrderAsync(string search, DateTime? dateFrom, DateTime? dateTo, int status, int page, int pageSize, string filterByDate, int type)
	{
		var query = _context.Orders.AsQueryable();

		// Filter by search keyword (GuestPhone)
		if (!string.IsNullOrEmpty(search))
		{
			search = search.ToLower();
			query = query.Where(d => d.GuestPhone.ToLower().Contains(search));
		}

		// Filter by DateFrom and DateTo based on filterBy parameter
		if (dateFrom != null || dateTo != null)
		{
			if (filterByDate == "Đặt hàng")
			{
				if (dateFrom != null && dateTo != null)
				{
					query = query.Where(d => d.OrderDate >= dateFrom && d.OrderDate <= dateTo.Value.AddDays(1).AddTicks(-1));
				}
				else if (dateFrom != null)
				{
					query = query.Where(d => d.OrderDate >= dateFrom && d.OrderDate <= dateFrom.Value.AddDays(1).AddTicks(-1));
				}
				else if (dateTo != null)
				{
					query = query.Where(d => d.OrderDate <= dateTo.Value.AddDays(1).AddTicks(-1));
				}
			}
			else if (filterByDate == "Giao hàng")
			{
				if (dateFrom != null && dateTo != null)
				{
					query = query.Where(d => d.RecevingOrder >= dateFrom && d.RecevingOrder <= dateTo.Value.AddDays(1).AddTicks(-1));
				}
				else if (dateFrom != null)
				{
					query = query.Where(d => d.RecevingOrder >= dateFrom && d.RecevingOrder <= dateFrom.Value.AddDays(1).AddTicks(-1));
				}
				else if (dateTo != null)
				{
					query = query.Where(d => d.RecevingOrder <= dateTo.Value.AddDays(1).AddTicks(-1));
				}
			}
		}

		// Filter by Status
		if (status != 0) // Assuming 0 means no filter by status
		{
			query = query.Where(d => (int)d.Status == status);
		}

		// Filter by Type
		if (type != 0)
		{
			query = query.Where(d => d.Type == type);
		}

		var totalOrders = await query.CountAsync();

		var orders = await query
			.Include(a => a.Address)
			.Include(o => o.OrderTables).ThenInclude(ot => ot.Table)
			.Include(d => d.Discount)
			.Include(i => i.Invoice)
			.OrderByDescending(o => filterByDate == "Đặt hàng" ? o.OrderDate : o.RecevingOrder)
			.Skip((page - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync();

		var orderDTOs = orders.Select(o => new OrderDTO
		{
			OrderId = o.OrderId,
			OrderDate = (DateTime)o.OrderDate,
			Status = (int)o.Status,
			RecevingOrder = o.RecevingOrder,
			AccountId = o.AccountId,
			TableIds = o.OrderTables.Select(tb => new TableAllDTO
			{
				TableId = tb.TableId,
				Capacity = tb.Table != null ? tb.Table.Capacity : default(int),
				Floor = tb.Table != null ? tb.Table.Floor : default(int),
				Status = tb.Table != null ? tb.Table.Status : default(int),
			}).ToList(),
			InvoiceId = o.InvoiceId,
			TotalAmount = o.TotalAmount,
			GuestPhone = o.GuestPhone,
			Deposits = (decimal)o.Deposits,
			AddressId = o.AddressId ?? 0,
			GuestAddress = o.Address?.GuestAddress,
			ConsigneeName = o.Address?.ConsigneeName,
			PaymentStatus = o.Invoice != null ? o.Invoice.PaymentStatus : default(int),
			Note = o.Note,
			Type = o.Type,
			DiscountId = o.DiscountId
		}).ToList();

		return new PagedResult<OrderDTO>(orderDTOs, totalOrders, page, pageSize);
	}




	public async Task<Order> UpdateOrderStatusAsync(int orderId, int status)
	{
		var od = await _context.Orders.FindAsync(orderId);
		if (od == null)
		{
			return null;
		}

		od.Status = status;
		_context.Orders.Update(od);
		await _context.SaveChangesAsync();

		return od;
	}

	public async Task<IEnumerable<Order>> GetOrdersWithTablesAsync()
	{
		return await _context.Orders
			.Include(o => o.OrderTables)
			.ThenInclude(ot => ot.Table)
			.Include(o => o.Address)
			.ToListAsync();
	}

	public async Task<Order?> GetOrderByTableIdAsync(int tableId)
	{
		return await _context.OrderTables
			.Where(ot => ot.TableId == tableId && ot.Order.Status == 3)
			.Include(ot => ot.Order)
				.ThenInclude(o => o.OrderDetails)
					.ThenInclude(od => od.Dish)
						.ThenInclude(d => d.Discount)
			.Include(ot => ot.Order)
				.ThenInclude(o => o.OrderDetails)
					.ThenInclude(od => od.Combo)
			.Include(ot => ot.Order)
				.ThenInclude(o => o.Address)
			.Include(ot => ot.Order)
				.ThenInclude(o => o.GuestPhoneNavigation)
			.Include(ot => ot.Table)
			.Select(ot => ot.Order)
			.FirstOrDefaultAsync();
	}



	public async Task UpdateOrderAsync(Order order)
	{
		_context.Orders.Update(order);
		await _context.SaveChangesAsync();
	}

	public async Task<Address?> GetOrCreateAddress2(CheckoutDTO checkoutDTO)
	{
		if (string.IsNullOrWhiteSpace(checkoutDTO.GuestAddress) ||
			string.IsNullOrWhiteSpace(checkoutDTO.ConsigneeName) ||
			string.IsNullOrWhiteSpace(checkoutDTO.GuestPhone))
		{
			return null;
		}

		var address = await _context.Addresses
			.FirstOrDefaultAsync(a =>
				a.GuestAddress == checkoutDTO.GuestAddress &&
				a.ConsigneeName == checkoutDTO.ConsigneeName &&
				a.GuestPhone == checkoutDTO.GuestPhone);

		if (address == null)
		{
			address = new Address
			{
				GuestAddress = checkoutDTO.GuestAddress,
				ConsigneeName = checkoutDTO.ConsigneeName,
				GuestPhone = checkoutDTO.GuestPhone
			};
			_context.Addresses.Add(address);
		}

		await _context.SaveChangesAsync();
		return address;
	}
	public async Task<Order> UpdateOrderForTable(int tableId, UpdateTableAndGetOrderDTO dto)
	{
		var order = await GetOrderByTableIdAsync(tableId);
		if (order == null)
		{
			throw new KeyNotFoundException($"Không tìm thấy đơn hàng cho bàn {tableId}.");
		}

		order.OrderDetails ??= new List<OrderDetail>();

		foreach (var detailDto in dto.OrderDetails)
		{
			if (detailDto.Quantity == 102)
			{
				// Xóa tất cả các OrderDetail có DishId hoặc ComboId tương ứng
				if (detailDto.DishId.HasValue && detailDto.DishId != 0)
				{
					var dishId = Math.Abs(detailDto.DishId.Value);
					var existingDetails = order.OrderDetails
						.Where(od => od.DishId == dishId)
						.ToList();

					foreach (var detail in existingDetails)
					{
						_context.OrderDetails.Remove(detail);
					}
				}
				else if (detailDto.ComboId.HasValue && detailDto.ComboId != 0)
				{
					var comboId = Math.Abs(detailDto.ComboId.Value);
					var existingDetails = order.OrderDetails
						.Where(od => od.ComboId == comboId)
						.ToList();

					foreach (var detail in existingDetails)
					{
						_context.OrderDetails.Remove(detail);
					}
				}
			}
			else if (detailDto.Quantity < 0)
			{
				// Xử lý khi quantity âm (giảm số lượng món ăn hoặc combo)
				if (detailDto.DishId.HasValue && detailDto.DishId != 0)
				{
					var dishId = Math.Abs(detailDto.DishId.Value);
					var existingDetails = order.OrderDetails
						.Where(od => od.DishId == dishId)
						.OrderByDescending(od => od.OrderTime)
						.ToList();

					var quantityToRemove = Math.Abs(detailDto.Quantity);
					foreach (var detail in existingDetails)
					{
						if (quantityToRemove <= 0) break;

						if (detail.Quantity <= quantityToRemove)
						{
							quantityToRemove -= detail.Quantity.Value;
							_context.OrderDetails.Remove(detail);
						}
						else
						{
							detail.Quantity -= quantityToRemove;
							quantityToRemove = 0;
						}
					}

					if (quantityToRemove > 0)
					{
						throw new InvalidOperationException($"Không đủ số lượng món ăn {dishId} để giảm.");
					}
				}
				else if (detailDto.ComboId.HasValue && detailDto.ComboId != 0)
				{
					var comboId = Math.Abs(detailDto.ComboId.Value);
					var existingDetails = order.OrderDetails
						.Where(od => od.ComboId == comboId)
						.OrderByDescending(od => od.OrderTime)
						.ToList();

					var quantityToRemove = Math.Abs(detailDto.Quantity);
					foreach (var detail in existingDetails)
					{
						if (quantityToRemove <= 0) break;

						if (detail.Quantity <= quantityToRemove)
						{
							quantityToRemove -= detail.Quantity.Value;
							_context.OrderDetails.Remove(detail);
						}
						else
						{
							detail.Quantity -= quantityToRemove;
							quantityToRemove = 0;
						}
					}

					if (quantityToRemove > 0)
					{
						throw new InvalidOperationException($"Không đủ số lượng combo {comboId} để giảm.");
					}
				}
			}
			else if (detailDto.Quantity > 0 && detailDto.Quantity <= 100)
			{
				// Xử lý khi quantity dương (thêm món ăn hoặc combo)
				if (detailDto.DishId.HasValue && detailDto.DishId != 0)
				{
					var dishId = Math.Abs(detailDto.DishId.Value);
					var dishExists = await _dishRepository.DishExistsAsync(dishId);
					if (!dishExists)
					{
						throw new KeyNotFoundException($"Món ăn {dishId} không tồn tại.");
					}

					var dish = await _dishRepository.GetByIdAsync(dishId);
					decimal unitPrice = dish.Discount != null
						? (decimal)(dish.Price - (dish.Price * dish.Discount.DiscountPercent / 100))
						: (decimal)dish.Price;

					order.OrderDetails.Add(new OrderDetail
					{
						OrderId = order.OrderId,
						DishId = dishId,
						ComboId = null,
						Quantity = detailDto.Quantity,
						UnitPrice = unitPrice * detailDto.Quantity,
						DishesServed = 0,
						Note = detailDto.Note,
						OrderTime = DateTime.Now
					});
				}
				else if (detailDto.ComboId.HasValue && detailDto.ComboId != 0)
				{
					var comboId = Math.Abs(detailDto.ComboId.Value);
					var comboExists = await _comboRepository.ComboExistsAsync(comboId);
					if (!comboExists)
					{
						throw new KeyNotFoundException($"Combo {comboId} không tồn tại.");
					}

					var combo = await _comboRepository.GetByIdAsync(comboId);
					decimal unitPrice = (decimal)combo.Price;

					order.OrderDetails.Add(new OrderDetail
					{
						OrderId = order.OrderId,
						DishId = null,
						ComboId = comboId,
						Quantity = detailDto.Quantity,
						UnitPrice = unitPrice * detailDto.Quantity,
						DishesServed = 0,
						Note = detailDto.Note,
						OrderTime = DateTime.Now
					});
				}
			}
		}

		decimal totalAmount = (decimal)order.OrderDetails.Sum(od => od.UnitPrice);

		decimal discountAmount = 0;
		if (dto.DiscountId.HasValue)
		{
			var discount = await _discountRepository.GetDiscountByIdAsync(dto.DiscountId.Value);
			if (discount != null && discount.DiscountStatus == true)
			{
				if (totalAmount >= (discount.TotalMoney ?? 0) &&
					(discount.QuantityLimit == null || discount.QuantityLimit > 0))
				{
					if (discount.DiscountPercent.HasValue)
					{
						discountAmount = (totalAmount * discount.DiscountPercent.Value) / 100;
					}

					if (discount.QuantityLimit.HasValue && discount.QuantityLimit > 0)
					{
						discount.QuantityLimit--;
					}
				}
			}
		}

		var finalTotalAmount = totalAmount - discountAmount;

		order.TotalAmount = finalTotalAmount;
		order.DiscountId = dto.DiscountId.HasValue && dto.DiscountId.Value > 0 ? dto.DiscountId.Value : (int?)null;

		await UpdateOrderAsync(order);
		return order;
	}


	//Update OrderDetail by OrderID
	public async Task<Order?> UpdateOrderDetailsByOrderId(int orderId, UpdateTableAndGetOrderDTO dto)
	{
		var order = await _context.Orders
			.Include(o => o.OrderDetails)
			.FirstOrDefaultAsync(o => o.OrderId == orderId);

		if (order == null)
		{
			throw new KeyNotFoundException($"Không tìm thấy đơn hàng với ID {orderId}.");
		}

		order.OrderDetails ??= new List<OrderDetail>();

		foreach (var detailDto in dto.OrderDetails)
		{
			if (detailDto.Quantity == 102)
			{
				// Xóa tất cả các OrderDetail có DishId hoặc ComboId tương ứng
				if (detailDto.DishId.HasValue && detailDto.DishId != 0)
				{
					var dishId = Math.Abs(detailDto.DishId.Value);
					var existingDetails = order.OrderDetails
						.Where(od => od.DishId == dishId)
						.ToList();

					foreach (var detail in existingDetails)
					{
						_context.OrderDetails.Remove(detail);
					}
				}
				else if (detailDto.ComboId.HasValue && detailDto.ComboId != 0)
				{
					var comboId = Math.Abs(detailDto.ComboId.Value);
					var existingDetails = order.OrderDetails
						.Where(od => od.ComboId == comboId)
						.ToList();

					foreach (var detail in existingDetails)
					{
						_context.OrderDetails.Remove(detail);
					}
				}
			}
			else if (detailDto.Quantity < 0)
			{
				if (detailDto.DishId.HasValue && detailDto.DishId != 0)
				{
					var dishId = Math.Abs(detailDto.DishId.Value);
					var existingDetails = order.OrderDetails
						.Where(od => od.DishId == dishId)
						.OrderByDescending(od => od.OrderTime)
						.ToList();

					var quantityToRemove = Math.Abs(detailDto.Quantity);
					foreach (var detail in existingDetails)
					{
						if (quantityToRemove <= 0) break;

						if (detail.Quantity <= quantityToRemove)
						{
							quantityToRemove -= detail.Quantity.Value;
							_context.OrderDetails.Remove(detail);
						}
						else
						{
							detail.Quantity -= quantityToRemove;
							quantityToRemove = 0;
						}
					}

					if (quantityToRemove > 0)
					{
						throw new InvalidOperationException($"Không đủ số lượng món ăn {dishId} để giảm.");
					}
				}
				else if (detailDto.ComboId.HasValue && detailDto.ComboId != 0)
				{
					var comboId = Math.Abs(detailDto.ComboId.Value);
					var existingDetails = order.OrderDetails
						.Where(od => od.ComboId == comboId)
						.OrderByDescending(od => od.OrderTime)
						.ToList();

					var quantityToRemove = Math.Abs(detailDto.Quantity);
					foreach (var detail in existingDetails)
					{
						if (quantityToRemove <= 0) break;

						if (detail.Quantity <= quantityToRemove)
						{
							quantityToRemove -= detail.Quantity.Value;
							_context.OrderDetails.Remove(detail);
						}
						else
						{
							detail.Quantity -= quantityToRemove;
							quantityToRemove = 0;
						}
					}

					if (quantityToRemove > 0)
					{
						throw new InvalidOperationException($"Không đủ số lượng combo {comboId} để giảm.");
					}
				}
			}
			else if (detailDto.Quantity > 0 && detailDto.Quantity <= 100)
			{
				if (detailDto.DishId.HasValue && detailDto.DishId != 0)
				{
					var dishId = Math.Abs(detailDto.DishId.Value);
					var dishExists = await _dishRepository.DishExistsAsync(dishId);
					if (!dishExists)
					{
						throw new KeyNotFoundException($"Món ăn {dishId} không tồn tại.");
					}

					var dish = await _dishRepository.GetByIdAsync(dishId);
					decimal unitPrice = dish.Discount != null
						? (decimal)(dish.Price - (dish.Price * dish.Discount.DiscountPercent / 100))
						: (decimal)dish.Price;

					order.OrderDetails.Add(new OrderDetail
					{
						OrderId = order.OrderId,
						DishId = dishId,
						ComboId = null,
						Quantity = detailDto.Quantity,
						UnitPrice = unitPrice * detailDto.Quantity,
						DishesServed = 0,
						Note = detailDto.Note,
						OrderTime = DateTime.Now
					});
				}
				else if (detailDto.ComboId.HasValue && detailDto.ComboId != 0)
				{
					var comboId = Math.Abs(detailDto.ComboId.Value);
					var comboExists = await _comboRepository.ComboExistsAsync(comboId);
					if (!comboExists)
					{
						throw new KeyNotFoundException($"Combo {comboId} không tồn tại.");
					}

					var combo = await _comboRepository.GetByIdAsync(comboId);
					decimal unitPrice = (decimal)combo.Price;

					order.OrderDetails.Add(new OrderDetail
					{
						OrderId = order.OrderId,
						DishId = null,
						ComboId = comboId,
						Quantity = detailDto.Quantity,
						UnitPrice = unitPrice * detailDto.Quantity,
						DishesServed = 0,
						Note = detailDto.Note,
						OrderTime = DateTime.Now
					});
				}
			}
		}

		decimal totalAmount = (decimal)order.OrderDetails.Sum(od => od.UnitPrice);

		order.TotalAmount = totalAmount;
		order.DiscountId = dto.DiscountId.HasValue && dto.DiscountId.Value > 0 ? dto.DiscountId.Value : (int?)null;

		await UpdateOrderAsync(order);
		return order;
	}

	public async Task<Order> CreateOrderForTable(int tableId, CreateOrderForTableDTO dto)
	{
		Guest guest = null;
		Address address = null;

		if (!string.IsNullOrWhiteSpace(dto.GuestPhone))
		{
			guest = await _context.Guests
				.FirstOrDefaultAsync(g => g.GuestPhone == dto.GuestPhone);
			if (guest == null)
			{
				guest = new Guest
				{
					GuestPhone = dto.GuestPhone
				};
				_context.Guests.Add(guest);
				await _context.SaveChangesAsync();
			}

			if (!string.IsNullOrWhiteSpace(dto.GuestAddress) && !string.IsNullOrWhiteSpace(dto.ConsigneeName))
			{
				address = await GetOrCreateAddress2(new CheckoutDTO
				{
					GuestAddress = dto.GuestAddress,
					ConsigneeName = dto.ConsigneeName,
					GuestPhone = dto.GuestPhone
				});
			}
		}

		var order = new Order
		{
			OrderDate = DateTime.Now,
			Status = dto.Status,
			RecevingOrder = dto.RecevingOrder,
			GuestPhone = !string.IsNullOrWhiteSpace(dto.GuestPhone) ? dto.GuestPhone : null,
			Note = dto.Note,
			Type = dto.Type,
			DiscountId = dto.DiscountId.HasValue && dto.DiscountId.Value > 0 ? dto.DiscountId.Value : (int?)null,
			Address = address,
			GuestPhoneNavigation = guest
		};

		if (string.IsNullOrWhiteSpace(dto.GuestPhone))
		{
			order.GuestPhone = null;
			order.GuestPhoneNavigation = null;
		}

		_context.Orders.Add(order);
		await _context.SaveChangesAsync();

		decimal totalAmount = 0m;

		if (dto.OrderDetails != null && dto.OrderDetails.Any())
		{
			foreach (var detailDto in dto.OrderDetails)
			{
				decimal unitPrice = 0m;

				if (detailDto.DishId.HasValue && detailDto.DishId.Value > 0)
				{
					var dish = await _context.Dishes
						.Include(d => d.Discount)
						.FirstOrDefaultAsync(d => d.DishId == detailDto.DishId.Value);

					if (dish == null)
					{
						throw new KeyNotFoundException($"Món ăn {detailDto.DishId} không tồn tại.");
					}

					if (dish.Discount != null)
					{
						unitPrice = (decimal)(dish.Price - (dish.Price * dish.Discount.DiscountPercent / 100)) * detailDto.Quantity;
					}
					else
					{
						unitPrice = (decimal)dish.Price * detailDto.Quantity;
					}

					var orderDetail = new OrderDetail
					{
						UnitPrice = unitPrice,
						Quantity = detailDto.Quantity,
						DishId = detailDto.DishId.Value,
						ComboId = null,
						OrderId = order.OrderId,
						DishesServed = 0,
						Note = detailDto.Note,
						OrderTime = DateTime.Now
					};

					_context.OrderDetails.Add(orderDetail);
				}
				else if (detailDto.ComboId.HasValue && detailDto.ComboId.Value > 0)
				{
					var combo = await _context.Combos
						.FirstOrDefaultAsync(c => c.ComboId == detailDto.ComboId.Value);

					if (combo == null)
					{
						throw new KeyNotFoundException($"Combo {detailDto.ComboId} không tồn tại.");
					}

					unitPrice = (decimal)combo.Price * detailDto.Quantity;

					var orderDetail = new OrderDetail
					{
						UnitPrice = unitPrice,
						Quantity = detailDto.Quantity,
						DishId = null,
						ComboId = detailDto.ComboId.Value,
						OrderId = order.OrderId,
						DishesServed = 0,
						Note = detailDto.Note,
						OrderTime = detailDto.OrderTime
					};

					_context.OrderDetails.Add(orderDetail);
				}
				else
				{
					continue;
				}

				totalAmount += unitPrice;
			}

			await _context.SaveChangesAsync();
		}

		var orderTable = new OrderTable
		{
			TableId = tableId,
			OrderId = order.OrderId
		};

		_context.OrderTables.Add(orderTable);
		await _context.SaveChangesAsync();

		order.TotalAmount = totalAmount;

		await _context.SaveChangesAsync();
		await _tableRepository.UpdateTableStatus(tableId, 1);
		return new Order
		{
			OrderId = order.OrderId,
			OrderDate = order.OrderDate,
			Status = order.Status,
			TotalAmount = order.TotalAmount,
			GuestPhone = order.GuestPhone,
			Note = order.Note
		};
	}

	public async Task UpdateOrderStatusForTableAsync(int tableId, int orderId, UpdateOrderStatusForTableDTO dto)
	{
		var order = await _context.Orders
			.Include(o => o.Address)
			.FirstOrDefaultAsync(o => o.OrderId == orderId);

		if (order == null)
		{
			throw new KeyNotFoundException($"Đơn hàng {orderId} không tìm thấy.");
		}

		var table = await _context.Tables.FirstOrDefaultAsync(t => t.TableId == tableId);
		if (table == null)
		{
			throw new KeyNotFoundException($"Bàn {tableId} không tìm thấy.");
		}

		order.Status = 3;
		order.RecevingOrder = dto.RecevingOrder ?? order.RecevingOrder;

		table.Status = 0;

		var invoice = new Invoice
		{
			PaymentTime = dto.PaymentTime,
			PaymentAmount = order.TotalAmount,
			Taxcode = dto.Taxcode,
			PaymentStatus = 1,
			CustomerName = order.Address?.ConsigneeName,
			Phone = order.Address?.GuestPhone,
			Address = order.Address?.GuestAddress,
			AccountId = dto.AccountId ?? null
		};

		_context.Invoices.Add(invoice);
		await _context.SaveChangesAsync();

		var invoiceLog = new InvoiceLog
		{
			Description = dto.Description,
			InvoiceId = invoice.InvoiceId
		};

		_context.InvoiceLogs.Add(invoiceLog);
		await _context.SaveChangesAsync();

		order.InvoiceId = invoice.InvoiceId;

		await _context.SaveChangesAsync();
	}

	public async Task CancelOrderAsync(int tableId, int orderId, CancelOrderDTO dto)
	{
		var order = await _context.Orders
			.Include(o => o.OrderDetails)
			.FirstOrDefaultAsync(o => o.OrderId == orderId);

		if (order == null)
		{
			throw new KeyNotFoundException($"Đơn hàng {orderId} không tìm thấy.");
		}

		var table = await _context.Tables.FirstOrDefaultAsync(t => t.TableId == tableId);
		if (table == null)
		{
			throw new KeyNotFoundException($"Bàn {tableId} không tìm thấy.");
		}

		bool allDishesNotServed = order.OrderDetails.All(od => od.DishesServed == null || od.DishesServed == 0);
		if (allDishesNotServed)
		{
			order.Status = 5;

		}

		if (dto.RecevingOrder.HasValue)
		{
			order.RecevingOrder = dto.RecevingOrder;
		}

		table.Status = 0;

		await _context.SaveChangesAsync();
	}


	public async Task<IEnumerable<OrderDetail>> GetOrderDetailsByOrderIdAsync(int orderId)
	{
		return await _context.OrderDetails
			.Include(od => od.Dish)
			.Include(od => od.Combo)
			.Where(od => od.OrderId == orderId)
			.ToListAsync();
	}


	public async Task<IEnumerable<Order>> GetOrdersByTableIdAsync(int tableId)
	{
		var orders = await _context.Orders
			.Where(o => _context.OrderTables.Any(ot => ot.TableId == tableId && ot.OrderId == o.OrderId) && o.Status == 3)
			.Include(o => o.OrderDetails)
			.ToListAsync();

		return orders;
	}

	public async Task<int> CountOrderByDiscountIdAsync(int discountId)
	{
		return await _context.Orders
			.CountAsync(order => order.DiscountId == discountId);
	}
	public async Task<IEnumerable<Order>> GetOrderDetailsForStaffType1Async()
	{
		return await _context.Orders
			.Include(o => o.OrderDetails)
				.ThenInclude(od => od.Dish)
			.Include(o => o.OrderDetails)
				.ThenInclude(od => od.Combo)
			.Include(o => o.OrderTables)
			.Include(o => o.Address)
			.Where(o =>
				((o.Status == 6))
				&& ((o.Type == 2 && o.RecevingOrder.HasValue && o.RecevingOrder.Value.Date == DateTime.Today)
				|| (o.Type == 1 && (o.RecevingOrder.HasValue && o.RecevingOrder.Value.Date == DateTime.Today || o.RecevingOrder == null))))
			.ToListAsync();
	}
    public async Task<Order?> UpdateCancelationReasonAsync(int orderId, string? reason)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order == null)
        {
            return null; 
        }
		order.Status = 5;
        order.CancelationReason = reason;
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();

        return order;
    }
}
