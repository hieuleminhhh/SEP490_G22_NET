﻿using AutoMapper;
using EHM_API.DTOs.CartDTO.Guest;
using EHM_API.DTOs.DishDTO;
using EHM_API.DTOs.DishDTO.Manager;
using EHM_API.DTOs.HomeDTO;
using EHM_API.DTOs.OrderDetailDTO.Manager;
using EHM_API.DTOs.OrderDTO.Cashier;
using EHM_API.DTOs.OrderDTO.Cashier.EHM_API.DTOs.OrderDTO;
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
            // Lấy order từ cơ sở dữ liệu
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                return null;
            }

            // Kết hợp chi tiết order
            var combinedOrderDetails = CombineOrderDetails(order.OrderDetails);
            var orderDto = _mapper.Map<OrderDTOAll>(order);
            orderDto.OrderDetails = _mapper.Map<IEnumerable<OrderDetailDTO>>(combinedOrderDetails);

            // Lấy thông tin Staff (nếu có)
            if (order.StaffId.HasValue)
            {
                var staff = await _context.Accounts.FindAsync(order.StaffId.Value);
                if (staff != null)
                {
                    orderDto.StaffId = staff.AccountId;
                    orderDto.StaffFirstName = staff.FirstName;
                    orderDto.StaffLastName = staff.LastName;
                }
            }

            // Lấy thông tin AcceptBy (nếu có)
            if (order.AcceptBy.HasValue)
            {
                var acceptBy = await _context.Accounts.FindAsync(order.AcceptBy.Value);
                if (acceptBy != null)
                {
                    orderDto.AcceptBy = acceptBy.AccountId;
                    orderDto.AcceptByFirstName = acceptBy.FirstName;
                    orderDto.AcceptByLastName = acceptBy.LastName;
                }
            }

            // Xử lý nếu là loại order có Reservation
            if (order.Type == 3 && order.Reservations != null && order.Reservations.Any())
            {
                orderDto.Reservation = _mapper.Map<ReservationDTOByOrderId>(order.Reservations.First());
            }

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
					.ThenInclude(o => o.Discount)
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

		public Task<Order> CreateOrderForReservation(int tableId, CreateOrderForReservaionDTO dto)
		{
			return _orderRepository.CreateOrderForReservation(tableId, dto);
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

            // Check if order date equals today's date
            var dateNow = DateTime.Now.Date; // Get today's date
            if (order.RecevingOrder.Value.Date == dateNow)
            {
                order.Status = 6;
            }
            else
            {
                order.Status = 2;
            }

            // Update the order status
            await _orderRepository.UpdateOrderAsync(order);

            var invoice = new Invoice
            {
                PaymentTime = dto.PaymentTime,
                PaymentAmount = dto.PaymentAmount,
                Taxcode = dto.Taxcode,
                PaymentStatus = dto.PaymentStatus,

                CustomerName = order.Address?.ConsigneeName,
                Phone = order.Address?.GuestPhone,
                Address = order.Address?.GuestAddress,

                AccountId = dto.AccountId != 0 ? dto.AccountId : (int?)null,
                AmountReceived = dto.AmountReceived,
                ReturnAmount = dto.ReturnAmount,
                PaymentMethods = dto.PaymentMethods,
                InvoiceLogs = new List<InvoiceLog>
        {
            new InvoiceLog { Description = dto.Description }
        }
            };
            await _invoiceRepository.CreateInvoiceAsync(invoice);

            // Update the order with the new invoice ID
            order.InvoiceId = invoice.InvoiceId;
            await _orderRepository.UpdateOrderAsync(order);

            return invoice.InvoiceId;
        }

        public async Task<IEnumerable<OrderDetailForStaffType1>> GetOrderDetailsForStaffType1Async()
		{
			var orders = await _orderRepository.GetOrderDetailsForStaffType1Async();

			var mappedOrders = _mapper.Map<IEnumerable<OrderDetailForStaffType1>>(orders);

			foreach (var order in mappedOrders)
			{

				if (order.TotalAmount > 0 && order.DiscountPercent.HasValue)
				{
					var discountPercentage = order.DiscountPercent.Value / 100m;
					order.DiscountedPrice = order.TotalAmount * (1 - discountPercentage);
				}
				else
				{
					order.DiscountedPrice = order.TotalAmount;
				}
			}

			return mappedOrders;
		}




		public async Task UpdateAmountReceivingAsync(int orderId, UpdateAmountReceiving dto)
		{
			var order = await _orderRepository.GetByIdAsync(orderId);
			if (order == null)
			{
				throw new KeyNotFoundException($"Không tìm thấy đơn hàng với OrderID {orderId}");
			}

			if (dto.Status.HasValue)
			{
				order.Status = dto.Status.Value;
			}

			if (dto.AmountReceived.HasValue)
			{
				order.Invoice!.AmountReceived = dto.AmountReceived.Value;
				order.Invoice!.PaymentStatus = 1;

				var invoiceLog = new InvoiceLog
				{
					Description = dto.Description,
					InvoiceId = order.InvoiceId
				};

				await _invoiceRepository.CreateInvoiceLog(invoiceLog);
			}

			await _orderRepository.UpdateOrderAsync(order);
		}

		public async Task<CancelationReasonDTO?> UpdateCancelationReasonAsync(int orderId, CancelationReasonDTO? cancelationReasonDTO)
		{
			if (cancelationReasonDTO == null)
			{
				throw new ArgumentNullException(nameof(cancelationReasonDTO));
			}

			var order = await _orderRepository.UpdateCancelationReasonAsync(orderId, cancelationReasonDTO.CancelationReason, cancelationReasonDTO.CancelBy);
			if (order == null)
			{
				return null;
			}

			var orderTables = await _orderRepository.GetOrderTablesByOrderIdAsync(orderId);
			foreach (var orderTable in orderTables)
			{
				var table = await _tableRepository.GetByIdAsync(orderTable.TableId);
				if (table != null)
				{
					table.Status = 0;
					await _tableRepository.UpdateTableAsync(table);
				}
			}

			return new CancelationReasonDTO
			{
				CancelationReason = order.CancelationReason,
                CancelBy = order.CancelBy
            };

		}

		public async Task AcceptOrderAsync(int orderId, AcceptOrderDTO acceptOrderDto)
		{
			var order = await _orderRepository.GetByIdAsync(orderId);
			if (order == null)
			{
				throw new Exception("Không tìm thấy đơn hàng");
			}

			if (order.RecevingOrder.HasValue && order.RecevingOrder.Value.Date == DateTime.Now.Date)
			{
				order.Status = 6;
			}
			else
			{
				order.Status = 2;
			}

			var invoice = _mapper.Map<Invoice>(acceptOrderDto);

			if (order.Address != null)
			{
				invoice.CustomerName = order.Address.ConsigneeName;
				invoice.Phone = order.Address.GuestPhone;
				invoice.Address = order.Address.GuestAddress;
			}


			if (order.AccountId.HasValue)
			{
				invoice.AccountId = order.AccountId;
			}
			else
			{
				invoice.AccountId = null;
			}

			invoice.PaymentTime = DateTime.Now;
			invoice.PaymentStatus = order.Deposits > 0 ? 1 : 0;

			await _invoiceRepository.CreateInvoiceAsync(invoice);

			order.InvoiceId = invoice.InvoiceId;
			await _orderRepository.UpdateOrderAsync(order);

			var invoiceLog = new InvoiceLog
			{
				InvoiceId = invoice.InvoiceId,
				Description = acceptOrderDto.Description
			};
			await _invoiceRepository.CreateInvoiceLog(invoiceLog);
		}
		public async Task<OrderAccountDTO?> UpdateAccountIdAsync(int orderId, UpdateOrderAccountDTO updateOrderAccountDTO)
		{
			var order = await _orderRepository.GetOrderById(orderId);
			if (order == null)
			{
				return null;
			}

			if (updateOrderAccountDTO.StaffId.HasValue)
			{
				order.StaffId = updateOrderAccountDTO.StaffId.Value;
				await _orderRepository.UpdateOrderShipTimeAsync(order);
			}

			return _mapper.Map<OrderAccountDTO>(order);
		}
		public async Task<IEnumerable<OrderDetailForStaffType1>> GetOrdersByStatusAndAccountIdAsync(int status, int staffId)
		{
			var orders = await _orderRepository.GetOrdersByStatusAndAccountIdAsync(status, staffId);
			return _mapper.Map<IEnumerable<OrderDetailForStaffType1>>(orders);
		}
		public async Task<Order> UpdateForOrderStatusAsync(int orderId, int status)
		{
			var order = await _orderRepository.UpdateOrderStatusAsync(orderId, status);
			return order;
		}
        public async Task<List<CollectedByStatisticsDTO>> GetOrderStatisticsAsync(DateTime? startDate, DateTime? endDate, int? collectedById)
        {
            return await _orderRepository.GetOrderStatisticsByCollectedByAsync(startDate, endDate, collectedById);
        }

        public async Task<IEnumerable<CategorySalesDTO>> GetSalesByCategoryAsync(DateTime? startDate, DateTime? endDate)
		{
			var salesByCategory = await _orderRepository.GetSalesByCategoryAsync(startDate, endDate);
			var categories = await _context.Categories.ToListAsync();

			var salesDtoList = categories.Select(category => new CategorySalesDTO
			{
				CategoryId = category.CategoryId,
				CategoryName = category.CategoryName,
				TotalSales = salesByCategory.ContainsKey(category.CategoryId) ? salesByCategory[category.CategoryId] : 0
			});

			return salesDtoList;
		}
		public async Task<List<ExportOrderDTO?>> GetOrderDetailsByIdsAsync(List<int> orderIds)
		{
			var orders = await _orderRepository.GetOrderByIdAsync(orderIds);
			if (orders == null || !orders.Any())
			{
				return new List<ExportOrderDTO?>();
			}

			return _mapper.Map<List<ExportOrderDTO?>>(orders);
		}
        public async Task<UpdateTotalAmountDTO?> UpdateTotalAmountAsync(int orderId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                return null;
            }

            decimal totalAmount = 0;

            foreach (var orderDetail in order.OrderDetails)
            {
                decimal unitPrice = 0;

                if (orderDetail.Dish != null)
                {
                   
                    decimal dishPrice = orderDetail.Dish.Price ?? 0;

               
                    if (orderDetail.Dish.DiscountId.HasValue)
                    {
                        var discount = orderDetail.Dish.Discount;
                        if (discount != null && discount.DiscountStatus == true && discount.Type == 2)
                        {
                            var discountPercent = discount.DiscountPercent ?? 0;
                            dishPrice -= dishPrice * discountPercent / 100;
                        }
                    }

                 
                    unitPrice = dishPrice * (orderDetail.Quantity ?? 1);
                }
                else if (orderDetail.Combo != null)
                {
                   
                    decimal comboPrice = orderDetail.Combo.Price ?? 0;
                    unitPrice = comboPrice * (orderDetail.Quantity ?? 1);
                }

              
                orderDetail.UnitPrice = unitPrice;

                
                totalAmount += unitPrice;
            }

            order.TotalAmount = totalAmount;

            await _orderRepository.UpdateOrderAsync(order);

            return new UpdateTotalAmountDTO
            {
                OrderId = order.OrderId,
                TotalAmount = totalAmount
            };
        }
        public async Task<OrderEmailDTO> GetEmailByOrderIdAsync(int orderId)
        {
            var order = await _orderRepository.GetOrderById1Async(orderId);
            if (order == null)
            {
                throw new Exception("Order not found.");
            }

            string? email = null;

       
            if (order.GuestPhoneNavigation != null)
            {
                email = order.GuestPhoneNavigation.Email;
            }

            var orderEmailDto = new OrderEmailDTO
            {
                OrderId = order.OrderId,
                Email = email
            };

            return orderEmailDto;
        }
        public async Task<bool> UpdateStaffByOrderIdAsync(UpdateStaffDTO updateStaffDTO)
        {
            var order = await _orderRepository.GetOrderByIdAsync(updateStaffDTO.OrderId);
            if (order == null)
            {
                return false;
            }

            order.StaffId = updateStaffDTO.StaffId;
            await _orderRepository.UpdateOrderAsync(order);
            await _orderRepository.SaveAsync();

            return true;
        }
        public async Task<List<OrderDetailWithStaffDTO>> GetOrdersWithStatus8Async()
        {
            // Chỉ lấy các order có Status = 8
            var orders = await _orderRepository.GetOrdersByStatusAsync(8);

            return orders.Select(o => new OrderDetailWithStaffDTO
            {
                OrderId = o.OrderId,
                OrderDate = o.OrderDate,
                Status = o.Status,
                RecevingOrder = o.RecevingOrder,
                TotalAmount = o.TotalAmount,
                GuestPhone = o.GuestPhone,
                Deposits = o.Deposits,
                Note = o.Note,
                CancelationReason = o.CancelationReason,
                ShipTime = o.ShipTime,
                AccountFirstName = o.Account?.FirstName,
                AccountLastName = o.Account?.LastName,
                StaffFirstName = o.Staff?.FirstName,
                StaffLastName = o.Staff?.LastName,
                Address = o.Address != null ? new AddressWithStaffDTO
                {
                    GuestAddress = o.Address.GuestAddress,
                    ConsigneeName = o.Address.ConsigneeName,
                    GuestPhone = o.Address.GuestPhone
                } : null,
                Discount = o.Discount != null ? new DiscountWithStaffDTO
                {
                    DiscountId = o.Discount.DiscountId,
                    DiscountAmount = o.Discount?.TotalMoney // Sử dụng TotalMoney thay cho DiscountAmount
                } : null,
                OrderDetails = o.OrderDetails.Select(od => new OrderDetailStaffDTO
                {
                    OrderDetailId = od.OrderDetailId,
                    UnitPrice = od.UnitPrice,
                    Quantity = od.Quantity,
                    DishName = od.Dish?.ItemName, // Sử dụng ItemName thay cho DishName
                    ComboName = od.Combo?.NameCombo, // Sử dụng NameCombo thay cho ComboName
                    Note = od.Note
                }).ToList(),
                Reservations = o.Reservations.Select(r => new ReservationWithStaffDTO
                {
                    ReservationId = r.ReservationId,
                    ReservationTime = r.ReservationTime,
                    GuestNumber = r.GuestNumber
                }).ToList()
            }).ToList();
        }
        public async Task<bool> UpdateAcceptByAsync(UpdateAcceptByDTO dto)
        {
            return await _orderRepository.UpdateAcceptByAsync(dto);
        }

    }
}