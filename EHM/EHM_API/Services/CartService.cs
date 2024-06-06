using EHM_API.DTOs.CartDTO;
using EHM_API.Models;
using EHM_API.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EHM_API.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IComboRepository _comboRepository;

        public CartService(ICartRepository cartRepository, IComboRepository comboRepository)
        {
            _cartRepository = cartRepository;
            _comboRepository = comboRepository;
        }

        public List<Cart2DTO> GetCart()
        {
            return _cartRepository.GetCart();
        }

        public async Task AddToCart(Cart2DTO orderDTO)
        {
            await _cartRepository.AddToCart(orderDTO);
        }

        public async Task UpdateCart(UpdateCartDTO updateCartDTO)
        {
            await _cartRepository.UpdateCart(updateCartDTO);
        }

        public void ClearCart()
        {
            _cartRepository.ClearCart();
        }

        public async Task Checkout(CheckoutDTO checkoutDTO)
        {
            Guest guest = await _cartRepository.GetOrCreateGuest(checkoutDTO);

            Address address = await _cartRepository.GetOrCreateAddress(checkoutDTO);

            checkoutDTO.AddressId = address.AddressId;

            decimal totalAmount = 0;

            var orderDetails = new List<OrderDetail>();

            foreach (var item in checkoutDTO.CartItems)
            {
                if ((item.DishId.HasValue && item.DishId.Value > 0) && (item.ComboId.HasValue && item.ComboId.Value > 0) ||
                    (!item.DishId.HasValue || item.DishId.Value <= 0) && (!item.ComboId.HasValue || item.ComboId.Value <= 0))
                {
                    throw new InvalidOperationException("Each CartItem must have either DishId or ComboId, but not both or neither.");
                }

                Dish dish = null;
                Combo combo = null;

                if (item.DishId.HasValue && item.DishId.Value > 0)
                {
                    dish = await _cartRepository.GetDishByIdAsync(item.DishId.Value);
                    if (dish == null)
                    {
                        throw new KeyNotFoundException($"Dish with ID {item.DishId} not found.");
                    }
                }

                if (item.ComboId.HasValue && item.ComboId.Value > 0)
                {
                    combo = await _comboRepository.GetComboByIdAsync(item.ComboId.Value);
                    if (combo == null)
                    {
                        throw new KeyNotFoundException($"Combo with ID {item.ComboId} not found.");
                    }
                }

                decimal? unitPrice = dish?.Price ?? combo?.Price;
                if (unitPrice == null)
                {
                    throw new InvalidOperationException("Either DishId or ComboId must be provided.");
                }

                var total = unitPrice * (item.Quantity ?? 0);

                var orderDetail = new OrderDetail
                {
                    DishId = dish != null ? (int?)dish.DishId : null,
                    ComboId = combo != null ? (int?)combo.ComboId : null,
                    Quantity = item.Quantity,
                    UnitPrice = unitPrice,
                    Note = item.Note
                };

                orderDetails.Add(orderDetail);
                totalAmount += total ?? 0m;
            }

            var order = new Order
            {
                OrderDate = checkoutDTO.OrderDate ?? DateTime.Now,
                Status = checkoutDTO.Status ?? 1,
                RecevingOrder = checkoutDTO.RecevingOrder,
                GuestPhone = guest.GuestPhone,
                TotalAmount = totalAmount,
                OrderDetails = orderDetails,
                Deposits = checkoutDTO.Deposits

            };

            await _cartRepository.CreateOrder(order);
        }



        public async Task<CheckoutSuccessDTO> GetCheckoutSuccessInfoAsync(string guestPhone)
        {
            var order = await _cartRepository.GetOrderByGuestPhoneAsync(guestPhone);

            if (order == null)
            {
                return null;
            }

            var guestAddress = order.GuestPhoneNavigation?.Addresses?.FirstOrDefault();

            // Lấy danh sách OrderDetail và lọc ra các OrderDetail có Dish
            var orderDetails = order.OrderDetails
                .Where(od => od.DishId != null)
                .Select(od =>
                {
                    var dish = od.Dish;
                    return new OrderDetailDTO
                    {
                        DishId = dish?.DishId ?? 0,
                        ItemName = dish?.ItemName,
                        ItemDescription = dish?.ItemDescription,
                        Price = dish?.Price,
                        ImageUrl = dish?.ImageUrl,
                        CategoryName = dish?.Category?.CategoryName,
                        DiscountId = dish?.DiscountId
                    };
                }).ToList();

            // Lấy danh sách ComboDetail từ OrderDetail có Combo
            var comboDetails = order.OrderDetails
                .Where(od => od.ComboId != null)
                .Select(od =>
                {
                    var combo = od.Combo;
                    return new ComboDetailDTO
                    {
                        ComboId = combo.ComboId,
                        NameCombo = combo.NameCombo,
                        Price = combo.Price,
                        Note = combo.Note,
                        ImageUrl = combo.ImageUrl
                    };
                }).ToList();

            var checkoutSuccessDTO = new CheckoutSuccessDTO
            {
                GuestAddress = guestAddress?.GuestAddress,
                ConsigneeName = guestAddress?.ConsigneeName,
                GuestPhone = order.GuestPhone,
                ReceivingTime = order.RecevingOrder,
                Email = order.GuestPhoneNavigation?.Email,
                OrderDetails = orderDetails,
                ComboDetails = comboDetails
            };

            return checkoutSuccessDTO;
        }


    }
}
