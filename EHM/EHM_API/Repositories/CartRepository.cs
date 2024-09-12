using EHM_API.Controllers;
using EHM_API.DTOs.CartDTO.Guest;
using EHM_API.DTOs.CartDTO.OrderStaff;
using EHM_API.Models;
using EHM_API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EHM_API.Repositories
{
    public class CartRepository : ICartRepository
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IDishService _dishService;
		private readonly IDiscountRepository _discountRepository;
		private readonly IComboService _comboService;
		private readonly EHMDBContext _context;

		public CartRepository(IHttpContextAccessor httpContextAccessor, IDishService dishService, EHMDBContext context, IDiscountRepository discountRepository)
		{
			_httpContextAccessor = httpContextAccessor;
			_dishService = dishService;
			_context = context;
			_discountRepository = discountRepository;
		}

		public List<Cart2DTO> GetCart()
		{
			if (_httpContextAccessor.HttpContext.Session.Keys.Contains("Cart2"))
			{
				return _httpContextAccessor.HttpContext.Session.Get<List<Cart2DTO>>("Cart2") ?? new List<Cart2DTO>();
			}
			return new List<Cart2DTO>();
		}

		public async Task AddToCart(Cart2DTO orderDTO)
		{
			var dish = await _dishService.GetDishByIdAsync(orderDTO.DishId);
			//	var combo = await _comboService.GetComboWithDishesAsync()

			if (dish == null)
			{
				throw new KeyNotFoundException("Invalid dish ID.");
			}

			orderDTO.ItemName = dish.ItemName;
			orderDTO.ItemDescription = dish.ItemDescription;
			orderDTO.Price = dish.Price;
			orderDTO.UnitPrice = dish.Price;
			orderDTO.TotalAmount = dish.Price * orderDTO.Quantity;
			orderDTO.ImageUrl = dish.ImageUrl;
			orderDTO.CategoryId = dish.CategoryId;

			var cart = GetCart();

			var existingOrder = cart.FirstOrDefault(x => x.DishId == orderDTO.DishId);
			if (existingOrder != null)
			{
				existingOrder.Quantity += orderDTO.Quantity;
				existingOrder.TotalAmount += orderDTO.TotalAmount;
			}
			else
			{
				cart.Add(orderDTO);
			}

			_httpContextAccessor.HttpContext.Session.Set("Cart2", cart);
		}

		public async Task UpdateCart(UpdateCartDTO updateCartDTO)
		{
			var cart = _httpContextAccessor.HttpContext.Session.Get<List<Cart2DTO>>("Cart2");
			if (cart == null)
			{
				throw new KeyNotFoundException("Cart not found.");
			}

			var item = cart.FirstOrDefault(x => x.DishId == updateCartDTO.DishId);
			if (item == null)
			{
				throw new KeyNotFoundException("Item not found in cart.");
			}

			if (updateCartDTO.Quantity < 0)
			{
				item.Quantity += updateCartDTO.Quantity;
				item.TotalAmount = item.Price * item.Quantity;
			}
			else
			{
				item.Quantity = updateCartDTO.Quantity;
				item.TotalAmount = item.Price * item.Quantity;
			}

			if (item.Quantity <= 0)
			{
				cart.Remove(item);
			}

			_httpContextAccessor.HttpContext.Session.Set("Cart2", cart);
		}


		//checkout Order
		public async Task CreateOrder(Order order)
		{
			await _context.Orders.AddAsync(order);
			await _context.SaveChangesAsync();
		}

		public void ClearCart()
		{
			if (_httpContextAccessor.HttpContext.Session.Keys.Contains("Cart2"))
			{
				_httpContextAccessor.HttpContext.Session.Remove("Cart2");
			}
		}
		public async Task<Guest> GetOrCreateGuest(CheckoutDTO checkoutDTO)
		{
			if (string.IsNullOrWhiteSpace(checkoutDTO.GuestPhone))
			{
				throw new ArgumentException("GuestPhone cannot be empty.");
			}

			// Kiểm tra xem khách hàng đã tồn tại chưa
			var guest = await _context.Guests
				.FirstOrDefaultAsync(g => g.GuestPhone == checkoutDTO.GuestPhone);

			if (guest != null)
			{
				// Cập nhật thông tin email nếu khách hàng đã tồn tại
				guest.Email = checkoutDTO.Email;
			}
			else
			{
				// Thêm mới khách hàng nếu chưa tồn tại
				guest = new Guest
				{
					GuestPhone = checkoutDTO.GuestPhone,
					Email = checkoutDTO.Email
				};
				await _context.Guests.AddAsync(guest);
			}

			await _context.SaveChangesAsync();
			return guest;
		}

		public async Task<Address?> GetOrCreateAddress(CheckoutDTO checkoutDTO)
		{
			if (string.IsNullOrWhiteSpace(checkoutDTO.GuestAddress) ||
				string.IsNullOrWhiteSpace(checkoutDTO.ConsigneeName) ||
				string.IsNullOrWhiteSpace(checkoutDTO.GuestPhone))
			{
				return null;
			}

			var guest = await _context.Guests
				.FirstOrDefaultAsync(g => g.GuestPhone == checkoutDTO.GuestPhone);

			if (guest == null)
			{
				guest = new Guest
				{
					GuestPhone = checkoutDTO.GuestPhone,
					Email = checkoutDTO.Email 
				};
				await _context.Guests.AddAsync(guest);
				await _context.SaveChangesAsync();
			}

			var address = await _context.Addresses
				.FirstOrDefaultAsync(a =>
					a.GuestAddress == checkoutDTO.GuestAddress &&
					a.ConsigneeName == checkoutDTO.ConsigneeName &&
					a.GuestPhone == checkoutDTO.GuestPhone);

			if (address != null)
			{
				return address;
			}
			else
			{
				address = new Address
				{
					GuestAddress = checkoutDTO.GuestAddress,
					ConsigneeName = checkoutDTO.ConsigneeName,
					GuestPhone = checkoutDTO.GuestPhone
				};
				await _context.Addresses.AddAsync(address);
				await _context.SaveChangesAsync();

				return address;
			}
		}


		public async Task<Dish> GetDishByIdAsync(int? dishId)
		{
			if (dishId == null)
			{
				return null;
			}

			return await _context.Dishes.FirstOrDefaultAsync(d => d.DishId == dishId);
		}
        public async Task<Combo> GetComboByIdAsync(int? comboId)
        {
            if (comboId == null)
            {
                return null;
            }

            return await _context.Combos.FirstOrDefaultAsync(d => d.ComboId == comboId);
        }

        public async Task<Order> GetOrderByGuestPhoneAsync(string guestPhone)
		{
			if (string.IsNullOrWhiteSpace(guestPhone))
			{
				return null;
			}

			return await _context.Orders
				.Where(o => o.GuestPhone == guestPhone)
				.Include(o => o.GuestPhoneNavigation)
					.ThenInclude(g => g.Addresses)
				.Include(o => o.OrderDetails)
					.ThenInclude(od => od.Dish)
						.ThenInclude(d => d.Category)
				.Include(o => o.OrderDetails)
					.ThenInclude(od => od.Combo)
				.Include(o => o.OrderDetails)
					.ThenInclude(od => od.Dish)
						.ThenInclude(d => d.Discount)
				.Include(o => o.Discount)
				.OrderByDescending(o => o.OrderId)
				.FirstOrDefaultAsync();
		}


		public async Task CreateOrderTakeOut(Order order)
		{
			await _context.Orders.AddAsync(order);
			await _context.SaveChangesAsync();
		}

		public async Task<Guest> GetOrCreateGuestTakeOut(TakeOutDTO takeOutDTO)
		{
			if (string.IsNullOrWhiteSpace(takeOutDTO.Email) || string.IsNullOrWhiteSpace(takeOutDTO.GuestPhone))
			{
				return null;
			}

			var guest = await _context.Guests.FirstOrDefaultAsync(g => g.GuestPhone == takeOutDTO.GuestPhone);

			if (guest != null)
			{
				guest.Email = takeOutDTO.Email;
			}
			else
			{
				guest = new Guest
				{
					GuestPhone = takeOutDTO.GuestPhone,
					Email = takeOutDTO.Email
				};
				await _context.Guests.AddAsync(guest);
			}

			await _context.SaveChangesAsync();
			return guest;
		}

		public async Task<Address?> GetOrCreateAddressTakeOut(TakeOutDTO takeOutDTO)
		{
			if (string.IsNullOrWhiteSpace(takeOutDTO.GuestAddress) ||
				string.IsNullOrWhiteSpace(takeOutDTO.Email) ||
				string.IsNullOrWhiteSpace(takeOutDTO.ConsigneeName) ||
				string.IsNullOrWhiteSpace(takeOutDTO.GuestPhone))
			{
				return null;
			}
			var guest = await _context.Guests
				.FirstOrDefaultAsync(g => g.GuestPhone == takeOutDTO.GuestPhone);
			if (guest == null)
			{
				guest = new Guest
				{
					GuestPhone = takeOutDTO.GuestPhone,
					Email = takeOutDTO.Email
				};
				await _context.Guests.AddAsync(guest);
				await _context.SaveChangesAsync();
			}
			else
			{
				guest.Email = takeOutDTO.Email;
				_context.Guests.Update(guest);
				await _context.SaveChangesAsync();
			}

			var address = await _context.Addresses.FirstOrDefaultAsync(a =>
				a.GuestAddress == takeOutDTO.GuestAddress &&
				a.ConsigneeName == takeOutDTO.ConsigneeName &&
				a.GuestPhone == takeOutDTO.GuestPhone);

			if (address == null)
			{
				address = new Address
				{
					GuestAddress = takeOutDTO.GuestAddress,
					ConsigneeName = takeOutDTO.ConsigneeName,
					GuestPhone = takeOutDTO.GuestPhone
				};
				await _context.Addresses.AddAsync(address);
				await _context.SaveChangesAsync();
			}

			return address;
		}

		public async Task<int> TakeOut(TakeOutDTO takeOutDTO)
		{
			Guest guest = null;
			Address address = null;

			if (!string.IsNullOrWhiteSpace(takeOutDTO.GuestPhone) && !string.IsNullOrWhiteSpace(takeOutDTO.Email))
			{
				guest = await GetOrCreateGuestTakeOut(takeOutDTO);
			}

			if (!string.IsNullOrWhiteSpace(takeOutDTO.GuestAddress) && !string.IsNullOrWhiteSpace(takeOutDTO.ConsigneeName))
			{
				address = await GetOrCreateAddressTakeOut(takeOutDTO);
				takeOutDTO.AddressId = address?.AddressId;
			}

			decimal totalAmount = 0;
			var orderDetails = new List<OrderDetail>();

			foreach (var item in takeOutDTO.OrderDetails)
			{
				if ((item.DishId.HasValue && item.DishId.Value > 0) && (item.ComboId.HasValue && item.ComboId.Value > 0) ||
					(!item.DishId.HasValue || item.DishId.Value <= 0) && (!item.ComboId.HasValue || item.ComboId.Value <= 0))
				{
					throw new InvalidOperationException("Mỗi giỏ hàng phải có một món hoặc Combo, không được có cả hai hoặc không có món nào.");
				}

				Dish dish = null;
				Combo combo = null;

				if (item.DishId.HasValue && item.DishId.Value > 0)
				{
					dish = await GetDishByIdAsync(item.DishId.Value);
					if (dish == null)
					{
						throw new KeyNotFoundException($"Món ăn với ID {item.DishId} không tồn tại.");
					}
				}

				if (item.ComboId.HasValue && item.ComboId.Value > 0)
				{
					combo = await GetComboByIdAsync(item.ComboId.Value);
					if (combo == null)
					{
						throw new KeyNotFoundException($"Combo với ID {item.ComboId} không tồn tại.");
					}
				}

				var existingOrderDetail = orderDetails.FirstOrDefault(od =>
					(dish != null && od.DishId == dish.DishId) ||
					(combo != null && od.ComboId == combo.ComboId));

				if (existingOrderDetail != null)
				{
					existingOrderDetail.Quantity += item.Quantity;
					existingOrderDetail.UnitPrice += item.UnitPrice;
					totalAmount += (item.UnitPrice ?? 0m);
				}
				else
				{
					var orderDetail = new OrderDetail
					{
						DishId = dish != null ? (int?)dish.DishId : null,
						ComboId = combo != null ? (int?)combo.ComboId : null,
						Quantity = item.Quantity,
						UnitPrice = item.UnitPrice,
						DishesServed = 0,
						OrderTime = DateTime.Now,
						Note = item.Note,
					};

					orderDetails.Add(orderDetail);
					totalAmount += (item.UnitPrice ?? 0m);
				}
			}

			var order = new Order
			{
				OrderDate = DateTime.Now,
				Status = takeOutDTO.Status ?? 0,
				RecevingOrder = takeOutDTO.RecevingOrder,
				AccountId = takeOutDTO.AccountId ?? null,
				GuestPhone = guest?.GuestPhone,
				TotalAmount = totalAmount,
				OrderDetails = orderDetails,
				Deposits = takeOutDTO.Deposits,
				AddressId = address?.AddressId,
				Note = takeOutDTO.Note,
				Type = takeOutDTO.Type,
				DiscountId = takeOutDTO.DiscountId
			};

		

			await _context.Orders.AddAsync(order);
			await _context.SaveChangesAsync();

			return order.OrderId;
		}
        public async Task<List<Order>> GetOrdersByAccountIdAsync(int accountId)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Dish)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Combo)
                .Include(o => o.Address)
                .Include(o => o.Reservations)
                .Where(o => o.AccountId == accountId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }


    }
}
