using AutoMapper;
using EHM_API.DTOs.DishDTO;
using EHM_API.DTOs.DishDTO.Manager;
using EHM_API.DTOs.HomeDTO;
using EHM_API.DTOs.OrderDTO.Guest;
using EHM_API.DTOs.OrderDTO.Manager;
using EHM_API.DTOs.TableDTO;
using EHM_API.DTOs.TableDTO.Manager;
using EHM_API.Models;
using EHM_API.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EHM_API.Services
{
	public class OrderService : IOrderService
	{
		private readonly IOrderRepository _orderRepository;
		private readonly IComboRepository _comboRepository;
		private readonly IDishRepository _dishRepository;
		private readonly IMapper _mapper;
		private readonly EHMDBContext _context;

		public OrderService(IOrderRepository orderRepository, IMapper mapper, EHMDBContext context, IComboRepository comboRepository)
		{
			_orderRepository = orderRepository;
			_mapper = mapper;
			_context = context;
			_comboRepository = comboRepository;
		}

		public async Task<IEnumerable<OrderDTOAll>> GetAllOrdersAsync()
		{
			var orders = await _orderRepository.GetAllAsync();
			var orderDtos = _mapper.Map<IEnumerable<OrderDTOAll>>(orders);
			return orderDtos;
		}

		public async Task<IEnumerable<SearchPhoneOrderDTO>> GetAllOrdersToSearchAsync()
		{
			var orders = await _orderRepository.GetAllAsync();
			var orderDtos = _mapper.Map<IEnumerable<SearchPhoneOrderDTO>>(orders);
			return orderDtos;
		}

		public async Task<OrderDTOAll> GetOrderByIdAsync(int id)
		{
			var order = await _orderRepository.GetByIdAsync(id);
			if (order == null)
			{
				return null;
			}

			var orderDto = _mapper.Map<OrderDTOAll>(order);
			orderDto.OrderDetails = _mapper.Map<IEnumerable<OrderDetailDTO>>(order.OrderDetails);

			return orderDto;
		}



		public async Task<IEnumerable<SearchPhoneOrderDTO>> SearchOrdersAsync(string guestPhone = null)
		{
			if (string.IsNullOrWhiteSpace(guestPhone))
			{
				return await GetAllOrdersToSearchAsync();
			}

			var orders = await _orderRepository.SearchAsync(guestPhone);
			var orderDtos = _mapper.Map<IEnumerable<SearchPhoneOrderDTO>>(orders);
			return orderDtos;
		}



		public async Task<OrderDTOAll> CreateOrderAsync(CreateOrderDTO createOrderDto)
		{

			var order = _mapper.Map<Order>(createOrderDto);


			var createdOrder = await _orderRepository.AddAsync(order);

			var orderDto = _mapper.Map<OrderDTOAll>(createdOrder);

			return orderDto;
		}

		public async Task<OrderDTOAll> UpdateOrderAsync(int id, UpdateOrderDTO updateOrderDto)
		{
			var existingOrder = await _orderRepository.GetByIdAsync(id);
			if (existingOrder == null)
			{
				return null;
			}

			_mapper.Map(updateOrderDto, existingOrder);

			var updatedOrder = await _orderRepository.UpdateAsync(existingOrder);
			return _mapper.Map<OrderDTOAll>(updatedOrder);
		}

		public async Task<bool> DeleteOrderAsync(int id)
		{
			var existingOrder = await _orderRepository.GetByIdAsync(id);
			if (existingOrder == null)
			{
				return false;
			}

			var orderDetails = _context.OrderDetails.Where(od => od.OrderId == id);
			_context.OrderDetails.RemoveRange(orderDetails);
			var result = await _orderRepository.DeleteAsync(id);

			await _context.SaveChangesAsync();

			return result;
		}

		public async Task<bool> CancelOrderAsync(int orderId)
		{
			var existingOrder = await _orderRepository.GetByIdAsync(orderId);
			if (existingOrder == null)
			{
				return false;
			}

			if (existingOrder.Status != 0)
			{
				return false;
			}

			existingOrder.Status = 4;
			await _orderRepository.UpdateAsync(existingOrder);
			return true;
		}
		public async Task<PagedResult<OrderDTO>> GetOrderAsync(string search, DateTime? dateFrom, DateTime? dateTo, int status, int page, int pageSize, string filterByDate, int type)
		{
			var pagedOrders = await _orderRepository.GetOrderAsync(search, dateFrom, dateTo, status, page, pageSize, filterByDate, type);
			var orderDTOs = _mapper.Map<IEnumerable<OrderDTO>>(pagedOrders.Items);

			foreach (var odDTO in orderDTOs)
			{
				if (odDTO.AccountId.HasValue)
				{
					var address = await _context.Addresses.FindAsync(odDTO.AccountId.Value);
					if (address != null)
					{
						odDTO.GuestAddress = address.GuestAddress;
					}
				}
			}
			return new PagedResult<OrderDTO>(orderDTOs, pagedOrders.TotalCount, pagedOrders.Page, pagedOrders.PageSize);
		}
		public async Task<Order> UpdateOrderStatusAsync(int comboId, int status)
		{
			return await _orderRepository.UpdateOrderStatusAsync(comboId, status);
		}


		//danh sách banh
		public async Task<IEnumerable<ListTableOrderDTO>> GetOrdersWithTablesAsync()
		{
			var orders = await _orderRepository.GetOrdersWithTablesAsync();
			return _mapper.Map<IEnumerable<ListTableOrderDTO>>(orders);
		}

		public async Task<FindTableAndGetOrderDTO?> GetOrderByTableIdAsync(int tableId)
		{
			var orderTable = await _context.OrderTables
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
				.FirstOrDefaultAsync(ot => ot.TableId == tableId);

			if (orderTable == null || orderTable.Order == null) return null;

			var order = orderTable.Order;
			var result = _mapper.Map<FindTableAndGetOrderDTO>(order);

			result.TableIds = order.OrderTables
				.Where(ot => ot.Table != null)
				.Select(ot => new GetTableDTO
				{
					TableId = ot.Table.TableId,
					Status = ot.Table.Status,
					Capacity = ot.Table.Capacity,
					Floor = ot.Table.Floor
				}).ToList();

			result.OrderDetails = (order.OrderDetails ?? new List<OrderDetail>())
				.Select(od => _mapper.Map<TableOfOrderDetailDTO>(od)).ToList();

			return result;
		}

		public async Task<Order?> UpdateOrderDetailsAsync(int tableId, UpdateTableAndGetOrderDTO dto)
		{
			var order = await _orderRepository.UpdateOrderForTable(tableId, dto);
			return order;
		}

		//Create

		public Task<Order> CreateOrderForTable(int tableId, CreateOrderForTableDTO dto)
		{
			return _orderRepository.CreateOrderForTable(tableId, dto);
		}

	}
}


