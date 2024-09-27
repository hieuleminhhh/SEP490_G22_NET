using AutoMapper;
using EHM_API.DTOs.CartDTO.OrderStaff;
using EHM_API.DTOs.InvoiceDTO;
using EHM_API.DTOs.OrderDTO.Manager;
using EHM_API.Models;
using EHM_API.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace EHM_API.Services
{
	public class InvoiceService : IInvoiceService
	{
		private readonly IInvoiceRepository _invoiceRepository;
		private readonly IOrderRepository _orderRepository;
		private readonly IMapper _mapper;

		public InvoiceService(IInvoiceRepository invoiceRepository, IMapper mapper, IOrderRepository orderRepository)
		{
			_invoiceRepository = invoiceRepository;
			_mapper = mapper;
			_orderRepository = orderRepository;
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
                        UnitPrice = g.Sum(item => item.UnitPrice ?? 0),  // Kiểm tra nullable
                        Quantity = g.Sum(item => item.Quantity ?? 0)  // Kiểm tra nullable
                    };
                })
                .ToList();
        }

        public async Task<int> CreateInvoiceForOrderAsync(int orderId, CreateInvoiceForOrderDTO createInvoiceDto)
        {
            return await _invoiceRepository.CreateInvoiceForOrderAsync(orderId, createInvoiceDto);
        }

		public async Task<int> CreateInvoiceForOrder(int orderId, CreateInvoiceForOrder2DTO createInvoiceDto)
		{
			return await _invoiceRepository.CreateInvoiceForOrder(orderId, createInvoiceDto);
		}

		public async Task UpdateInvoiceAndCreateGuestAsync(int invoiceId, UpdateInvoiceDTO dto)
		{
			await _invoiceRepository.UpdateInvoiceAndCreateGuestAsync(invoiceId, dto);
		}

		public async Task UpdateInvoiceAndOrderAsync(int orderId, UpdateInvoiceSuccessPaymentDTO dto)
		{
			var order = await _orderRepository.GetByIdAsync(orderId);
			if (order == null)
			{
				throw new KeyNotFoundException($"Không tìm thấy đơn hàng với ID {orderId}.");
			}

			var invoice = await _invoiceRepository.GetInvoiceByIdAsync(order.InvoiceId.Value);
			if (invoice == null)
			{
				throw new KeyNotFoundException($"Không tìm thấy hóa đơn với ID {order.InvoiceId}.");
			}

			invoice.PaymentStatus = dto.PaymentStatus;

			order.Status = dto.Status;

			invoice.AmountReceived = order.TotalAmount;

			await _invoiceRepository.UpdateInvoiceAsync(invoice);
			await _orderRepository.UpdateOrderAsync(order);
		}

		public async Task UpdateOrderStatusAsync(int orderId, UpdateStatusOrderDTO dto)
		{
			var order = await _orderRepository.GetByIdAsync(orderId);
			if (order == null)
			{
				throw new KeyNotFoundException($"Không tìm thấy đơn hàng với ID {orderId}.");
			}

			order.Status = dto.Status;


			await _orderRepository.UpdateOrderAsync(order);
		}

		public async Task<InvoiceDetailDTO> GetInvoiceByOrderIdAsync(int orderId)
		{
			var invoice = await _invoiceRepository.GetInvoiceByOrderIdAsync(orderId);
			if (invoice == null)
			{
				throw new KeyNotFoundException($"Không tìm thấy hóa đơn cho đơn hàng {orderId}");
			}

			var invoiceDetail = await _invoiceRepository.GetInvoiceDetailAsync(invoice.InvoiceId);
			if (invoiceDetail == null)
			{
				throw new KeyNotFoundException($"Không tìm thấy chi tiết hóa đơn cho hóa đơn {invoice.InvoiceId}");
			}

			invoiceDetail.ItemInvoice = CombineInvoiceItems(invoiceDetail.ItemInvoice);
			return invoiceDetail;
		}

		public async Task<int> UpdateDepositAndCreateInvoiceAsync(int orderId, PrepaymentDTO dto)
		{
			var order = await _orderRepository.GetByIdAsync(orderId);
			if (order == null)
			{
				throw new KeyNotFoundException($"Không tìm thấy đơn hàng {orderId}");
			}

			order.Status = 3;
			order.Deposits = dto.Deposits;
			

			var invoice = _mapper.Map<Invoice>(dto);

			if (order.Address != null)
			{
				invoice.CustomerName = order.Address.ConsigneeName;
				invoice.Phone = order.Address.GuestPhone;
				invoice.Address = order.Address.GuestAddress;
			}

			

			await _invoiceRepository.CreateInvoiceAsync(invoice);

			invoice.InvoiceLogs.Add(new InvoiceLog
			{
				Description = dto.Description,
				InvoiceId = invoice.InvoiceId
			});
			order.InvoiceId = invoice.InvoiceId;
			await _orderRepository.UpdateAsync(order);

			return invoice.InvoiceId;
		}

		public async Task UpdateOrderAndInvoiceAsync(int orderId, InvoiceOfSitting dto)
		{
			await _invoiceRepository.UpdateOrderAndInvoiceAsync(orderId, dto);
		}

		public async Task<IEnumerable<GetInvoiceAndOrderInfo>> GetAllInvoicesAndOrdersAsync()
		{
			var orders = await _invoiceRepository.GetAllOrdersWithInvoicesAsync();
			var invoiceAndOrderInfoList = _mapper.Map<IEnumerable<GetInvoiceAndOrderInfo>>(orders);
			return invoiceAndOrderInfoList;
		}

        public async Task<IEnumerable<GetOrderCancelInfo>> GetOrdersWithStatusAndDepositAsync(int status, decimal minDeposit)
        {
            // Lấy danh sách orders từ repository
            var orders = await _invoiceRepository.GetOrdersWithInvoicesByStatusAndDepositAsync(status, minDeposit);

            // Map từ Order sang DTO
            var invoiceAndOrderInfoList = orders.Select(order =>
            {
                var dto = _mapper.Map<GetOrderCancelInfo>(order);

                // Lấy thông tin Staff (FirstName, LastName) nếu có
                if (order.Staff != null)
                {
                    dto.StaffFirstName = order.Staff.FirstName;
                    dto.StaffLastName = order.Staff.LastName;
                }

            

                return dto;
            }).ToList();

            return invoiceAndOrderInfoList;
        }


        public async Task<IEnumerable<GetOrderUnpaidOfShipDTO>> GetOrdersUnpaidForShipAsync()
        {
            var orders = await _invoiceRepository.GetOrdersUnpaidForShipAsync();

            var orderDtos = orders.Select(order =>
            {
                var dto = _mapper.Map<GetOrderUnpaidOfShipDTO>(order);

                // Lấy thông tin Staff (FirstName, LastName)
                if (order.Staff != null)
                {
                    dto.FirstName = order.Staff.FirstName;
                    dto.LastName = order.Staff.LastName;
                }
                if (order.Collected != null)
                {
                    dto.CollectedFirstName = order.Collected.FirstName;
                    dto.CollectedLastName = order.Collected.LastName;
                }

                // Tính toán lại TotalPaid với Discount nếu có
                if (order.Discount != null && order.Discount.DiscountPercent.HasValue)
                {
                    var discountPercent = order.Discount.DiscountPercent.Value;
                    dto.TotalPaid = order.TotalAmount - (order.TotalAmount * discountPercent / 100);
                }
                else
                {
                    dto.TotalPaid = order.TotalAmount;
                }

                return dto;
            }).ToList();

            return orderDtos;
        }



        public async Task<bool> UpdatePaymentStatusAsync(int orderId, UpdatePaymentStatusDTO dto)
        {
            var order = await _invoiceRepository.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                return false; // Không tìm thấy đơn hàng
            }

            var invoice = order.Invoice;
            if (invoice == null)
            {
                return false; // Không tìm thấy hóa đơn
            }

            // Cập nhật trạng thái thanh toán
            invoice.PaymentStatus = 1;
            invoice.PaymentTime = DateTime.Now;
            invoice.PaymentAmount = dto.PaymentAmount;

            // Cập nhật người thu tiền trong đơn hàng
            order.CollectedBy = dto.CollectedBy;

            // Cập nhật tài khoản trong hóa đơn giống với CollectedBy
            invoice.AccountId = dto.CollectedBy; // Cập nhật AccountId

            // Cập nhật hóa đơn và đơn hàng
            await _invoiceRepository.UpdateInvoiceAsync(invoice);
            await _orderRepository.UpdateOrderAsync(order);

            return true; // Cập nhật thành công
        }


        public async Task<UpdateAmountInvoiceDTO?> UpdateInvoiceAsync(UpdateAmountInvoiceDTO updateAmountInvoiceDTO)
        {
            var invoice = await _invoiceRepository.GetInvoiceByIdAsync(updateAmountInvoiceDTO.InvoiceId);
            if (invoice == null)
            {
                return null;
            }

            
            invoice.PaymentAmount = updateAmountInvoiceDTO.PaymentAmount;
            invoice.ReturnAmount = updateAmountInvoiceDTO.ReturnAmount;

           
            await _invoiceRepository.UpdateInvoiceAsync(invoice);

            return _mapper.Map<UpdateAmountInvoiceDTO>(invoice);
        }

    }
}

