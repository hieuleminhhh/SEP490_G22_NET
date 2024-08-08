using AutoMapper;
using EHM_API.DTOs.DishDTO;
using EHM_API.DTOs.DishDTO.Manager;
using EHM_API.DTOs.HomeDTO;
using EHM_API.DTOs.OrderDetailDTO.Manager;
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
		private readonly IInvoiceRepository _invoiceRepository;
		private readonly ITableRepository _tableRepository;
		private readonly IMapper _mapper;
		private readonly EHMDBContext _context;

		public OrderService(IOrderRepository orderRepository, IMapper mapper, EHMDBContext context, IComboRepository comboRepository, ITableRepository tableRepository, IInvoiceRepository invoiceRepository)
		{
			_orderRepository = orderRepository;
			_mapper = mapper;
			_context = context;
			_comboRepository = comboRepository;
			_tableRepository = tableRepository;
			_invoiceRepository = invoiceRepository;
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

			var combinedOrderDetails = CombineOrderDetails(order.OrderDetails);
			var orderDto = _mapper.Map<OrderDTOAll>(order);
			orderDto.OrderDetails = _mapper.Map<IEnumerable<OrderDetailDTO>>(combinedOrderDetails);

			return orderDto;
		}

		private IEnumerable<OrderDetail> CombineOrderDetails(IEnumerable<OrderDetail> orderDetails)
		{
			return orderDetails
				.GroupBy(od => new { od.DishId, od.ComboId })
				.Select(g =>
				{
					var first = g.First();
					first.Quantity = g.Sum(od => od.Quantity);
					first.UnitPrice = g.Sum(od => od.UnitPrice);
					first.DishesServed = g.Sum(od => od.DishesServed);
					return first;
				})
				.ToList();
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
			existingOrder.Status = 5;
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
				.FirstOrDefaultAsync(ot => ot.TableId == tableId && ot.Order.Status == 3);

			if (orderTable == null || orderTable.Order == null) return null;

			var order = orderTable.Order;
			var combinedOrderDetails = CombineOrderDetails(order.OrderDetails);
			order.OrderDetails = combinedOrderDetails.ToList();

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

		public async Task<Order?> UpdateOrderDetailsByOrderIdAsync(int orderId, UpdateTableAndGetOrderDTO dto)
		{
			var order = await _orderRepository.UpdateOrderDetailsByOrderId(orderId, dto);
			return order;
		}


		//Create

		public Task<Order> CreateOrderForTable(int tableId, CreateOrderForTableDTO dto)
		{
			return _orderRepository.CreateOrderForTable(tableId, dto);
		}

		public async Task UpdateOrderStatusForTableAsync(int tableId, int orderId, UpdateOrderStatusForTableDTO dto)
		{
			await _orderRepository.UpdateOrderStatusForTableAsync(tableId, orderId, dto);
		}
		public async Task CancelOrderForTableAsync(int tableId, int orderId, CancelOrderDTO dto)
		{
			await _orderRepository.CancelOrderAsync(tableId, orderId, dto);
		}

		public async Task<IEnumerable<GetOrderDetailDTO>> GetOrderDetailsByOrderIdAsync(int orderId)
		{
			var orderDetails = await _orderRepository.GetOrderDetailsByOrderIdAsync(orderId);
			return _mapper.Map<IEnumerable<GetOrderDetailDTO>>(orderDetails);
		}

		public async Task<IEnumerable<GetDishOrderDetailDTO>> GetOrderDetailsByOrderId(int orderId)
		{
			var orderDetails = await _orderRepository.GetOrderDetailsByOrderIdAsync(orderId);

			var combinedOrderDetails = orderDetails
			.GroupBy(od => new { od.DishId, od.ComboId })
			.Select(g =>
			{
				var first = g.First();
				return new OrderDetail
				{
					ComboId = first.ComboId,
					DishId = first.DishId,
					Quantity = g.Sum(od => od.Quantity),
					Dish = first.Dish, 
					Combo = first.Combo
				};
			})
			.ToList();


			return _mapper.Map<IEnumerable<GetDishOrderDetailDTO>>(combinedOrderDetails);
		}




		public async Task UpdateOrderAndTablesStatusAsyncByTableId(int tableId, CancelOrderTableDTO dto)
		{
			var orders = await _orderRepository.GetOrdersByTableIdAsync(tableId);
			if (orders == null || !orders.Any())
			{
				throw new KeyNotFoundException($"Không tìm thấy đơn hàng cho bàn {tableId}.");
			}

			foreach (var order in orders)
			{
				if (order.OrderDetails.Any(od => od.DishesServed > 0))
				{
					throw new InvalidOperationException("Không thể huỷ đơn hàng vì đã có món ăn được phục vụ.");
				}

				if (dto.Status.HasValue)
				{
					order.Status = dto.Status.Value;
				}

				await _orderRepository.UpdateOrderAsync(order);
			}

			var table = await _tableRepository.GetTableByIdAsync(tableId);
			if (table != null)
			{
				table.Status = 0;
				await _tableRepository.UpdateTableAsync(table);
			}
		}
		public async Task<int> UpdateStatusAndCreateInvoiceAsync(int orderId, UpdateStatusAndCInvoiceD dto)
		{
			var order = await _orderRepository.GetByIdAsync(orderId);
			if (order == null)
			{
				throw new KeyNotFoundException($"Không tìm thấy đơn hàng {orderId}");
			}

			order.Status = dto.Status;
			await _orderRepository.UpdateOrderAsync(order);

			var invoice = new Invoice
			{
				PaymentTime = dto.PaymentTime,
				PaymentAmount = dto.PaymentAmount,
				Taxcode = dto.Taxcode,
				PaymentStatus = 1,

				CustomerName = order.Address?.ConsigneeName,
				Phone = order.Address?.GuestPhone,
				Address = order.Address?.GuestAddress,

			//	AccountId = dto.AccountId,
				AmountReceived = dto.AmountReceived,
				ReturnAmount = dto.ReturnAmount,
				PaymentMethods = dto.PaymentMethods,
				InvoiceLogs = new List<InvoiceLog>
		{
			new InvoiceLog { Description = dto.Description }
		}
			};
			await _invoiceRepository.CreateInvoiceAsync(invoice);

			order.InvoiceId = invoice.InvoiceId;
			await _orderRepository.UpdateOrderAsync(order);

			return invoice.InvoiceId;
		}
        public async Task<IEnumerable<OrderDetailForStaffType1>> GetOrderDetailsForStaffType1Async()
        {
            var orderDetails = await _orderRepository.GetOrderDetailsForStaffType1Async();
            return _mapper.Map<IEnumerable<OrderDetailForStaffType1>>(orderDetails);
        }


    }
}

	


