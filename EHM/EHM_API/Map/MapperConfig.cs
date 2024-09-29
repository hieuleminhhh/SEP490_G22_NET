using AutoMapper;
using EHM_API.DTOs.AccountDTO;
using EHM_API.DTOs.CartDTO.Guest;
using EHM_API.DTOs.CartDTO.OrderStaff;
using EHM_API.DTOs.CategoryDTO;
using EHM_API.DTOs.CategoryDTO.Guest;
using EHM_API.DTOs.CategoryDTO.Manager;
using EHM_API.DTOs.ComboDTO.EHM_API.DTOs.ComboDTO;
using EHM_API.DTOs.ComboDTO.Guest;
using EHM_API.DTOs.ComboDTO.Manager;
using EHM_API.DTOs.DiscountDTO.Manager;
using EHM_API.DTOs.DishDTO.Manager;
using EHM_API.DTOs.GuestDTO.Guest;
using EHM_API.DTOs.GuestDTO.Manager;
using EHM_API.DTOs.HomeDTO;
using EHM_API.DTOs.IngredientDTO.Manager;
using EHM_API.DTOs.InvoiceDTO;
using EHM_API.DTOs.MaterialDTO;
using EHM_API.DTOs.NewDTO;
using EHM_API.DTOs.NotificationDTO;
using EHM_API.DTOs.OrderDetailDTO.Manager;
using EHM_API.DTOs.OrderDTO.Cashier;
using EHM_API.DTOs.OrderDTO.Guest;
using EHM_API.DTOs.OrderDTO.Manager;
using EHM_API.DTOs.OrderTableDTO;
using EHM_API.DTOs.ReservationDTO.Guest;
using EHM_API.DTOs.ReservationDTO.Manager;
using EHM_API.DTOs.SettingDTO.Manager;
using EHM_API.DTOs.Table_ReservationDTO;
using EHM_API.DTOs.TableDTO;
using EHM_API.DTOs.TableDTO.Manager;
using EHM_API.DTOs.TBDTO;
using EHM_API.Models;
using EHM_API.Services;

namespace EHM_API.Map
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<CreateGuestDTO, Guest>()
                            .ForMember(dest => dest.GuestPhone, opt => opt.MapFrom(src => src.GuestPhone))
                            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                            .ForMember(dest => dest.Addresses, opt => opt.Ignore());

            CreateMap<CreateGuestDTO, Address>()
                .ForMember(dest => dest.GuestAddress, opt => opt.MapFrom(src => src.GuestAddress))
                .ForMember(dest => dest.ConsigneeName, opt => opt.MapFrom(src => src.ConsigneeName))
                .ForMember(dest => dest.GuestPhone, opt => opt.MapFrom(src => src.GuestPhone))
                .ForMember(dest => dest.GuestPhoneNavigation, opt => opt.Ignore());

            CreateMap<Dish, DishDTOAll>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.CategoryName : null))
                .ForMember(dest => dest.DiscountedPrice, opt => opt.MapFrom(
                    src => src.DiscountId == null ? (decimal?)null :
                           src.DiscountId == 0 ? src.Price :
                           src.Price.HasValue && src.Discount != null ? src.Price.Value - (src.Price.Value * src.Discount.DiscountPercent / 100) : (decimal?)null
                    ))
                .ForMember(dest => dest.DiscountPercentage, opt => opt.MapFrom(
                    src => src.DiscountId == null || src.DiscountId == 0 ? (int?)null : src.Discount != null ? src.Discount.DiscountPercent : (int?)null
                    ));

            CreateMap<CreateDishDTO, Dish>();
            CreateMap<UpdateDishDTO, Dish>();

            //Tim kiem Order
            CreateMap<Order, OrderDTOAll>()
                .ForMember(dest => dest.GuestPhone, opt => opt.MapFrom(src => src.Address != null ? src.Address.GuestPhone : null))
                .ForMember(dest => dest.GuestAddress, opt => opt.MapFrom(src => src.Address != null ? src.Address.GuestAddress : null))
                .ForMember(dest => dest.ConsigneeName, opt => opt.MapFrom(src => src.Address != null ? src.Address.ConsigneeName : null))
                .ForMember(dest => dest.AmountReceived, opt => opt.MapFrom(src => src.Invoice != null ? src.Invoice.AmountReceived : null))
                .ForMember(dest => dest.ReturnAmount, opt => opt.MapFrom(src => src.Invoice != null ? src.Invoice.ReturnAmount : null))
                 .ForMember(dest => dest.DiscountId, opt => opt.MapFrom(src => src.Discount != null ? src.Discount.DiscountId : (int?)null))
                .ForMember(dest => dest.DiscountPercent, opt => opt.MapFrom(src => src.Discount != null ? src.Discount.DiscountPercent : (int?)null))
                .ForMember(dest => dest.DiscountName, opt => opt.MapFrom(src => src.Discount != null ? src.Discount.DiscountName : null))
                .ForMember(dest => dest.QuantityLimit, opt => opt.MapFrom(src => src.Discount != null ? src.Discount.QuantityLimit : null))
                .ForMember(dest => dest.AmountReceived, opt => opt.MapFrom(src => src.Invoice != null ? src.Invoice.AmountReceived : (decimal?)null))
                .ForMember(dest => dest.ReturnAmount, opt => opt.MapFrom(src => src.Invoice != null ? src.Invoice.ReturnAmount : (decimal?)null))
                .ForMember(dest => dest.PaymentMethods, opt => opt.MapFrom(src => src.Invoice != null ? src.Invoice.PaymentMethods : (int?)null))
                .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.Invoice != null ? src.Invoice.PaymentStatus : (int?)null))
                .ForMember(dest => dest.PaymentTime, opt => opt.MapFrom(src => src.Invoice != null ? src.Invoice.PaymentTime : (DateTime?)null))
                .ForMember(dest => dest.Taxcode, opt => opt.MapFrom(src => src.Invoice != null ? src.Invoice.Taxcode : null))
                .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.Invoice != null ? src.Invoice.PaymentStatus : 0))
                 .ForMember(dest => dest.EmailOfAccount, opt => opt.MapFrom(src => src.Account != null ? src.Account.Email : null))
                  .ForMember(dest => dest.EmailOfGuest, opt => opt.MapFrom(src => src.GuestPhoneNavigation != null ? src.GuestPhoneNavigation.Email : null))
                .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.OrderDetails))
                .ForMember(dest => dest.Tables, opt => opt.MapFrom(src => src.OrderTables.Select(ot => ot.Table))).ReverseMap();


            CreateMap<OrderDetail, OrderDetailDTO>()
                .ForMember(dest => dest.DishId, opt => opt.MapFrom(src => src.DishId))
                .ForMember(dest => dest.ComboId, opt => opt.MapFrom(src => src.ComboId))
                .ForMember(dest => dest.NameCombo, opt => opt.MapFrom(src => src.Combo != null ? src.Combo.NameCombo : null))
                .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.Dish != null ? src.Dish.ItemName : null))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
                 .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Combo != null ? src.Combo.Price : (src.Dish != null ? src.Dish.Price : (decimal?)null)))
                .ForMember(dest => dest.DiscountedPrice, opt => opt.MapFrom(src =>
                    src.Dish != null && src.Dish.DiscountId != null && src.Dish.DiscountId != 0 && src.Dish.Price.HasValue ?
                    src.Dish.Price.Value - (src.Dish.Price.Value * (src.Dish.Discount != null ? src.Dish.Discount.DiscountPercent : 0) / 100) :
                    (decimal?)null))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Dish != null ? src.Dish.ImageUrl : src.Combo != null ? src.Combo.ImageUrl : null))
                .ForMember(dest => dest.DishesServed, opt => opt.MapFrom(src => src.DishesServed));


            CreateMap<OrderDetail, GetDishOrderDetailDTO>()
            .ForMember(dest => dest.ComboId, opt => opt.MapFrom(src => src.ComboId))
            .ForMember(dest => dest.DishId, opt => opt.MapFrom(src => src.DishId))
             .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.Dish != null ? src.Dish.ItemName : null))
            .ForMember(dest => dest.NameCombo, opt => opt.MapFrom(src => src.Combo != null ? src.Combo.NameCombo : null))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));


            CreateMap<Table, TableOfOrderDTO>();
            CreateMap<TableReservation, TableOfReservationDTO>()
               .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Table.Status))
               .ForMember(dest => dest.Capacity, opt => opt.MapFrom(src => src.Table.Capacity))
               .ForMember(dest => dest.Floor, opt => opt.MapFrom(src => src.Table.Floor))
               .ForMember(dest => dest.Lable, opt => opt.MapFrom(src => src.Table.Lable));

            CreateMap<CreateOrderDTO, Order>();
            CreateMap<UpdateOrderDTO, Order>();
            CreateMap<SearchOrdersRequestDTO, Order>();


            CreateMap<Order, SearchPhoneOrderDTO>()
                .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.OrderDetails))
                .ForMember(dest => dest.GuestAddress, opt => opt.MapFrom(src => src.Address.GuestAddress))
                .ForMember(dest => dest.ConsigneeName, opt => opt.MapFrom(src => src.Address.ConsigneeName))
                .ForMember(dest => dest.PaymentMethods, opt => opt.MapFrom(src =>
                    src.Deposits == 0 ? 1 :
                    src.Deposits == src.TotalAmount / 2 ? 2 :
                    src.Deposits == src.TotalAmount ? 3 : 0))
                .ReverseMap();



            // Mapping OrderDetail to OrderDetailsDTO
            CreateMap<OrderDetail, OrderDetailsDTO>()
                .ForMember(dest => dest.NameCombo, opt => opt.MapFrom(src => src.Combo != null ? src.Combo.NameCombo : null))
                .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.Dish != null ? src.Dish.ItemName : null))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Combo != null ? src.Combo.ImageUrl : (src.Dish != null ? src.Dish.ImageUrl : null)))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Combo != null && src.Combo.Price.HasValue ? src.Combo.Price : (src.Dish != null ? src.Dish.Price : (decimal?)null)))
                .ForMember(dest => dest.DiscountedPrice, opt => opt.MapFrom(src =>
                    src.Dish != null && src.Dish.Price.HasValue && src.Dish.Discount != null
                    ? src.Dish.Price.Value - (src.Dish.Price.Value * src.Dish.Discount.DiscountPercent / 100)
                    : src.Combo != null && src.Combo.Price.HasValue
                    ? src.Combo.Price.Value
                    : src.Dish != null ? src.Dish.Price.Value : (decimal?)null))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));

			// Mapping Order to CheckoutSuccessDTO
			CreateMap<Order, CheckoutSuccessDTO>()
			   .ForMember(dest => dest.GuestPhone, opt => opt.MapFrom(src => src.GuestPhone))
			   .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.GuestPhoneNavigation.Email))
			   .ForMember(dest => dest.AddressId, opt => opt.MapFrom(src => src.Address.AddressId))
			   .ForMember(dest => dest.GuestAddress, opt => opt.MapFrom(src => src.Address.GuestAddress))
			   .ForMember(dest => dest.ConsigneeName, opt => opt.MapFrom(src => src.Address.ConsigneeName))
			   .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderId))
			   .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate))
			   .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status ?? 0))
			   .ForMember(dest => dest.ReceivingTime, opt => opt.MapFrom(src => src.RecevingOrder))
			   .ForMember(dest => dest.DiscountPriceOrder, opt => opt.MapFrom(src =>
				   src.Discount != null && src.Discount.DiscountPercent.HasValue && src.TotalAmount.HasValue
					   ? (src.TotalAmount.Value * src.Discount.DiscountPercent.Value) / 100m : 0m))
			   .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src =>
				   src.TotalAmount.HasValue
					   ? src.TotalAmount.Value -
						 (src.Discount != null && src.Discount.DiscountPercent.HasValue
						   ? (src.TotalAmount.Value * src.Discount.DiscountPercent.Value) / 100m
						   : 0m)
					   : 0m))
			   .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
			   .ForMember(dest => dest.DiscountId, opt => opt.MapFrom(src => src.DiscountId))
			   .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Note))
			   .ForMember(dest => dest.Deposits, opt => opt.MapFrom(src => src.Deposits))
			   .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.OrderDetails));
			// Mapping Dish to OrderDetailsDTO
			CreateMap<Dish, OrderDetailsDTO>()
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.DiscountedPrice, opt => opt.MapFrom(src =>
                    src.DiscountId == null ? src.Price :
                    src.DiscountId == 0 ? src.Price :
                    src.Price.HasValue && src.Discount != null ? src.Price.Value - (src.Price.Value * src.Discount.DiscountPercent / 100) : src.Price))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.Price));

            // Mapping Combo to OrderDetailsDTO
            CreateMap<Combo, OrderDetailsDTO>()
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.DiscountedPrice, opt => opt.Ignore())
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.Price));



            //Guest DTO
            CreateMap<Address, GuestAddressInfoDTO>()
           .ForMember(dest => dest.GuestAddress, opt => opt.MapFrom(src => src.GuestAddress))
           .ForMember(dest => dest.ConsigneeName, opt => opt.MapFrom(src => src.ConsigneeName))
           .ForMember(dest => dest.GuestPhone, opt => opt.MapFrom(src => src.GuestPhone))
           .ForMember(dest => dest.Email, opt => opt.Ignore());



            CreateMap<Category, CategoryDTO>().ReverseMap();
            CreateMap<CreateCategory, Category>().ReverseMap();
            CreateMap<Category, ViewCategoryDTO>().ReverseMap();

            CreateMap<Combo, ComboDTO>().ReverseMap();

            CreateMap<ComboDetail, Dish>().ReverseMap();



            CreateMap<CreateComboDTO, Combo>().ReverseMap();


            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap<Combo, ViewComboDTO>()
             .ForMember(dest => dest.Dishes, opt => opt.MapFrom(src => src.ComboDetails.Select(cd => cd.Dish)));

            CreateMap<ComboDetail, DishDTO>()
                .ForMember(dest => dest.DishId, opt => opt.MapFrom(src => src.Dish.DishId))
                .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.Dish.ItemName))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Dish.Price));

            CreateMap<Ingredient, IngredientAllDTO>()
               .ForMember(dest => dest.DishItemName, opt => opt.MapFrom(src => src.Dish.ItemName))
               .ForMember(dest => dest.MaterialName, opt => opt.MapFrom(src => src.Material.Name))
               .ForMember(dest => dest.MaterialUnit, opt => opt.MapFrom(src => src.Material.Unit));

            CreateMap<CreateIngredientDTO, Ingredient>();
            CreateMap<UpdateIngredientDTO, Ingredient>()
                .ForMember(dest => dest.MaterialId, opt => opt.Ignore());

            CreateMap<Material, MaterialAllDTO>();
            CreateMap<CreateMaterialDTO, Material>().ReverseMap();
            CreateMap<UpdateMaterialDTO, Material>().ReverseMap();


            // Reservation to ReservationDetailDTO
            CreateMap<Reservation, ReservationDetailDTO>()
            .ForMember(dest => dest.ConsigneeName, opt =>
            {
                opt.MapFrom(src =>
                        src.Order != null && src.Order.Address != null ? src.Order.Address.ConsigneeName :
                        src.Address != null ? src.Address.ConsigneeName :
                        null);
            })
                .ForMember(dest => dest.GuestPhone,
                           opt => opt.MapFrom(src => src.Address.GuestPhone))
                .ForMember(dest => dest.ReservationTime,
                           opt => opt.MapFrom(src => src.ReservationTime))
                .ForMember(dest => dest.Status,
                           opt => opt.MapFrom(src => src.Status))
                 .ForMember(dest => dest.GuestAddress,
                           opt => opt.MapFrom(src => src.Address.GuestAddress))
                  .ForMember(dest => dest.Email,
              opt => opt.MapFrom(src => src.Address.GuestPhoneNavigation.Email))
                    .ForMember(dest => dest.TableOfReservation, opt => opt.MapFrom(src => src.TableReservations.Select(tr => new TabledetailDTO
                    {
                        TableId = tr.Table.TableId,
                        Capacity = tr.Table.Capacity,
                        Floor = tr.Table.Floor,
                        Lable = tr.Table.Lable
                    })))
                .ForMember(dest => dest.GuestNumber,
                           opt => opt.MapFrom(src => src.GuestNumber))
                .ForMember(dest => dest.Note,
                           opt => opt.MapFrom(src => src.Note))
                .ForMember(dest => dest.Order,
                           opt => opt.MapFrom(src => src.Order));

            CreateMap<UpdateStatusReservationTable, Reservation>()
           .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.ReservationStatus));

            CreateMap<UpdateStatusReservationTable, Table>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.TableStatus));

            // Map Order to OrderDetailDTO1
            CreateMap<Order, OrderDetailDTO1>()
                .ForMember(dest => dest.OrderId,
                           opt => opt.MapFrom(src => src.OrderId))
                .ForMember(dest => dest.OrderDate,
                           opt => opt.MapFrom(src => src.OrderDate))
                .ForMember(dest => dest.Status,
                           opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.TotalAmount,
                           opt => opt.MapFrom(src => src.TotalAmount))
                .ForMember(dest => dest.Note,
                           opt => opt.MapFrom(src => src.Note))
                .ForMember(dest => dest.OrderDetails,
                           opt => opt.MapFrom(src => src.OrderDetails));

            // Map OrderDetail to OrderItemDTO1 with conditional mapping
            CreateMap<OrderDetail, OrderItemDTO1>()
                .ForMember(dest => dest.DishId,
                           opt => opt.MapFrom(src => src.DishId ?? 0))
                .ForMember(dest => dest.ItemName,
                           opt => opt.MapFrom(src => src.DishId != null ? src.Dish.ItemName : null))
                .ForMember(dest => dest.ComboId,
                           opt => opt.MapFrom(src => src.ComboId ?? 0))
                .ForMember(dest => dest.NameCombo,
                           opt => opt.MapFrom(src => src.ComboId != null ? src.Combo.NameCombo : null))
                .ForMember(dest => dest.UnitPrice,
                           opt => opt.MapFrom(src => src.UnitPrice))
                .ForMember(dest => dest.Quantity,
                           opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.Price,
                       opt => opt.MapFrom(src => src.DishId != null && src.Dish.Price.HasValue ? src.Dish.Price : src.Combo.Price))
                .ForMember(dest => dest.DiscountedPrice,
                           opt => opt.MapFrom(src => CalculateDiscountedPrice(src)))
                 .ForMember(dest => dest.ImageUrl,
                       opt => opt.MapFrom(src => src.DishId != null ? src.Dish.ImageUrl : src.Combo.ImageUrl));

            // Map Combo to OrderItemDTO1 for Combo properties
            CreateMap<Combo, OrderItemDTO1>()
                .ForMember(dest => dest.ComboId,
                           opt => opt.MapFrom(src => src.ComboId))
                .ForMember(dest => dest.NameCombo,
                           opt => opt.MapFrom(src => src.NameCombo));


            //Table
            CreateMap<Table, TableAllDTO>();
            CreateMap<Table, FindTableDTO>()
               .ForMember(dest => dest.CombinedTables, opt => opt.Ignore());

            // 
            CreateMap<Table, TableAllDTO>();

            //danh sach ban cua orer
            CreateMap<Order, ListTableOrderDTO>()
            .ForMember(dest => dest.Tables, opt => opt.MapFrom(src => src.OrderTables.Select(ot => ot.Table).ToList()))
            .ForMember(dest => dest.GuestAddress, opt => opt.MapFrom(src => src.Address.GuestAddress))
            .ForMember(dest => dest.ConsigneeName, opt => opt.MapFrom(src => src.Address.ConsigneeName)); ;

            CreateMap<Table, TableOrderDTO>();

            CreateMap<RegisterTablesDTO, Reservation>()
               .ForMember(dest => dest.TableReservations, opt => opt.Ignore());
            //map address
            CreateMap<Address, GuestAddressInfoDTO>()
                .ForMember(dest => dest.GuestAddress, opt => opt.MapFrom(src => src.GuestAddress))
                .ForMember(dest => dest.ConsigneeName, opt => opt.MapFrom(src => src.ConsigneeName))
                .ForMember(dest => dest.GuestPhone, opt => opt.MapFrom(src => src.GuestPhone))
                .ForMember(dest => dest.Email, opt => opt.Ignore());
            //kiem tra dat ban
            CreateMap<Reservation, ReservationByStatus>()
                         .ForMember(dest => dest.ConsigneeName, opt => opt.MapFrom(src => src.Address != null ? src.Address.ConsigneeName : null))
                         .ForMember(dest => dest.GuestPhone, opt => opt.MapFrom(src => src.Address.GuestPhone))
                         .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Address != null && src.Address.GuestPhoneNavigation != null
          ? src.Address.GuestPhoneNavigation.Email
          : null))
                          .ForMember(dest => dest.GuestAddress, opt => opt.MapFrom(src => src.Address.GuestAddress))
                         .ForMember(dest => dest.ReservationTime, opt => opt.MapFrom(src => src.ReservationTime))
                         .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
              // Map TableReservations
              .ForMember(dest => dest.TableOfReservation, opt => opt.MapFrom(src => src.TableReservations.Select(tr => new TableReservationDTO
              {
                  TableId = tr.Table.TableId,
                  Capacity = tr.Table.Capacity,
                  Floor = tr.Table.Floor,
                  Lable = tr.Table.Lable
              })))
                         .ForMember(dest => dest.GuestNumber, opt => opt.MapFrom(src => src.GuestNumber))
                         .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Note))
                         .ForMember(dest => dest.Deposits, opt => opt.MapFrom(src => src.Order != null ? src.Order.Deposits : 0));



            // Map Order to OrderDetailDTO1
            CreateMap<Order, OrderDetailDTO3>()
                    .ForMember(dest => dest.OrderId,
                               opt => opt.MapFrom(src => src.OrderId))
                    .ForMember(dest => dest.OrderDate,
                               opt => opt.MapFrom(src => src.OrderDate))
                    .ForMember(dest => dest.Status,
                               opt => opt.MapFrom(src => src.Status))
                    .ForMember(dest => dest.TotalAmount,
                               opt => opt.MapFrom(src => src.TotalAmount))
                    .ForMember(dest => dest.Note,
                               opt => opt.MapFrom(src => src.Note))
                    .ForMember(dest => dest.OrderDetails,
                               opt => opt.MapFrom(src => src.OrderDetails));

            // Map OrderDetail to OrderItemDTO1 with conditional mapping
            CreateMap<OrderDetail, OrderItemDTO3>()
                .ForMember(dest => dest.DishId,
                           opt => opt.MapFrom(src => src.DishId ?? 0))
                .ForMember(dest => dest.ItemName,
                           opt => opt.MapFrom(src => src.DishId != null ? src.Dish.ItemName : null))
                .ForMember(dest => dest.ComboId,
                           opt => opt.MapFrom(src => src.ComboId ?? 0))
                .ForMember(dest => dest.NameCombo,
                           opt => opt.MapFrom(src => src.ComboId != null ? src.Combo.NameCombo : null))
                .ForMember(dest => dest.UnitPrice,
                           opt => opt.MapFrom(src => src.UnitPrice))
                .ForMember(dest => dest.Quantity,
                           opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.Price,
                       opt => opt.MapFrom(src => src.DishId != null && src.Dish.Price.HasValue ? src.Dish.Price : src.Combo.Price))
                .ForMember(dest => dest.DiscountedPrice,
                           opt => opt.MapFrom(src => CalculateDiscountedPrice(src)))
                 .ForMember(dest => dest.ImageUrl,
                       opt => opt.MapFrom(src => src.DishId != null ? src.Dish.ImageUrl : src.Combo.ImageUrl));

            // Map Combo to OrderItemDTO1 for Combo properties
            CreateMap<Combo, OrderItemDTO3>()
                .ForMember(dest => dest.ComboId,
                       opt => opt.MapFrom(src => src.ComboId))
                    .ForMember(dest => dest.NameCombo,
                       opt => opt.MapFrom(src => src.NameCombo));
            CreateMap<RegisterTablesDTO, Reservation>()
                  .ForMember(dest => dest.TableReservations, opt => opt.Ignore());
            //map address
            CreateMap<Address, GuestAddressInfoDTO>()
             .ForMember(dest => dest.GuestAddress, opt => opt.MapFrom(src => src.GuestAddress))
             .ForMember(dest => dest.ConsigneeName, opt => opt.MapFrom(src => src.ConsigneeName))
             .ForMember(dest => dest.GuestPhone, opt => opt.MapFrom(src => src.GuestPhone))
             .ForMember(dest => dest.Email, opt => opt.Ignore());

            //Search guest Name or Guest PHone to Reservation
            CreateMap<Reservation, ReservationSearchDTO>()
                           .ForMember(dest => dest.ConsigneeName, opt => opt.MapFrom(src => src.Address != null ? src.Address.ConsigneeName : null))
                       .ForMember(dest => dest.GuestPhone, opt => opt.MapFrom(src => src.Address.GuestPhone))
                       .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Address != null && src.Address.GuestPhoneNavigation != null ? src.Address.GuestPhoneNavigation.Email : null))
                        .ForMember(dest => dest.GuestAddress, opt => opt.MapFrom(src => src.Address.GuestAddress))
                       .ForMember(dest => dest.ReservationTime, opt => opt.MapFrom(src => src.ReservationTime))
                       .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                        .ForMember(dest => dest.TableOfReservation, opt => opt.MapFrom(src => src.TableReservations.Select(tr => new TableReservationDTO
                        {
                            TableId = tr.Table.TableId,
                            Capacity = tr.Table.Capacity,
                            Floor = tr.Table.Floor
                        })))
                       .ForMember(dest => dest.GuestNumber, opt => opt.MapFrom(src => src.GuestNumber))
                       .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Note))
                       .ForMember(dest => dest.Deposits, opt => opt.MapFrom(src => src.Order != null ? src.Order.Deposits : 0));


            // Map Order to OrderDetailDTO1
            CreateMap<Order, SearchOrderDetailDTO3>()
                    .ForMember(dest => dest.OrderId,
                               opt => opt.MapFrom(src => src.OrderId))
                    .ForMember(dest => dest.OrderDate,
                               opt => opt.MapFrom(src => src.OrderDate))
                    .ForMember(dest => dest.Status,
                               opt => opt.MapFrom(src => src.Status))
                    .ForMember(dest => dest.TotalAmount,
                               opt => opt.MapFrom(src => src.TotalAmount))
                    .ForMember(dest => dest.Note,
                               opt => opt.MapFrom(src => src.Note))
                    .ForMember(dest => dest.OrderDetails,
                               opt => opt.MapFrom(src => src.OrderDetails));
            // Map OrderDetail to OrderItemDTO1 with conditional mapping
            CreateMap<OrderDetail, SearchOrderItemDTO3>()
                .ForMember(dest => dest.DishId,
                           opt => opt.MapFrom(src => src.DishId ?? 0))
                .ForMember(dest => dest.ItemName,
                           opt => opt.MapFrom(src => src.DishId != null ? src.Dish.ItemName : null))
                .ForMember(dest => dest.ComboId,
                           opt => opt.MapFrom(src => src.ComboId ?? 0))
                .ForMember(dest => dest.NameCombo,
                           opt => opt.MapFrom(src => src.ComboId != null ? src.Combo.NameCombo : null))
                .ForMember(dest => dest.UnitPrice,
                           opt => opt.MapFrom(src => src.UnitPrice))
                .ForMember(dest => dest.Quantity,
                           opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.Price,
                       opt => opt.MapFrom(src => src.DishId != null && src.Dish.Price.HasValue ? src.Dish.Price : src.Combo.Price))
                .ForMember(dest => dest.DiscountedPrice,
                           opt => opt.MapFrom(src => CalculateDiscountedPrice(src)))
                 .ForMember(dest => dest.ImageUrl,
                       opt => opt.MapFrom(src => src.DishId != null ? src.Dish.ImageUrl : src.Combo.ImageUrl));

            CreateMap<Combo, SearchOrderItemDTO3>()
                .ForMember(dest => dest.ComboId,
                           opt => opt.MapFrom(src => src.ComboId))
                .ForMember(dest => dest.NameCombo,
                           opt => opt.MapFrom(src => src.NameCombo));
            //Het sear

            // Danh sach mon an and  combo 
            CreateMap<Dish, SearchDishDTO>()
            .ForMember(dest => dest.DiscountedPrice, opt => opt.MapFrom(
                    src => src.DiscountId == null ? (decimal?)null :
                           src.DiscountId == 0 ? src.Price :
                           src.Price.HasValue && src.Discount != null ? src.Price.Value - (src.Price.Value * src.Discount.DiscountPercent / 100) : (decimal?)null
                    ));

            CreateMap<Combo, SearchComboDTO>();

			CreateMap<Order, FindTableAndGetOrderDTO>()
	   .ForMember(dest => dest.GuestAddress, opt => opt.MapFrom(src => src.Address.GuestAddress))
	   .ForMember(dest => dest.ConsigneeName, opt => opt.MapFrom(src => src.Address.ConsigneeName))
	   .ForMember(dest => dest.GuestPhone, opt => opt.MapFrom(src => src.GuestPhone))
	   .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.OrderDetails))
	   .ForMember(dest => dest.TableIds, opt => opt.Ignore())
	   .ForMember(dest => dest.TotalDiscount, opt => opt.MapFrom(src =>
		   src.Discount != null && src.TotalAmount.HasValue
			   ? (src.Discount.DiscountPercent.HasValue
				   ? src.TotalAmount.Value - (src.TotalAmount.Value * src.Discount.DiscountPercent.Value / 100)
				   : src.TotalAmount.Value - (src.Discount.TotalMoney ?? 0))
			   : src.TotalAmount
	   ));


			CreateMap<OrderDetail, TableOfOrderDetailDTO>()
                .ForMember(dest => dest.Dish, opt => opt.MapFrom(src => src.Dish))
                .ForMember(dest => dest.Combo, opt => opt.MapFrom(src => src.Combo));

            CreateMap<OrderTable, GetTableDTO>()
                .ForMember(dest => dest.TableId, opt => opt.MapFrom(src => src.Table.TableId))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Table.Status))
                .ForMember(dest => dest.Capacity, opt => opt.MapFrom(src => src.Table.Capacity))
                .ForMember(dest => dest.Floor, opt => opt.MapFrom(src => src.Table.Floor));

            CreateMap<OrderDetail, OrderDetailForChefDTO>()
     .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Order.Type))
     .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.Dish.ItemName))
     .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
     .ForMember(dest => dest.OrderTime, opt => opt.MapFrom(src => src.OrderTime))
      .ForMember(dest => dest.RecevingOrder, opt => opt.MapFrom(src => src.Order.RecevingOrder))
     .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Note))
     .ForMember(dest => dest.DishesServed, opt => opt.MapFrom(src => src.DishesServed))
     .ForMember(dest => dest.ComboDetailsForChef, opt => opt.MapFrom(src => src.Combo.ComboDetails
         .GroupBy(cd => cd.ComboId)
         .Select(g => g.First())
         .Select(cd => new ComboDetailForChefDTO
         {
             ComboName = cd.Combo.NameCombo,
             ItemsInCombo = cd.Combo.ComboDetails.Select(item => new ItemInComboDTO
             {
                 ItemName = item.Dish.ItemName,
                 QuantityDish = item.QuantityDish
             }).ToList(),
             Note = cd.Combo.Note,
             OrderTime = src.OrderTime
         }).ToList()));

            CreateMap<OrderDetail, OrderDetailForChef1DTO>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Order.Type))
                .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.Dish.ItemName))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.OrderTime, opt => opt.MapFrom(src => src.OrderTime))
                .ForMember(dest => dest.RecevingOrder, opt => opt.MapFrom(src => src.Order.RecevingOrder))
                .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Note))
                .ForMember(dest => dest.DishesServed, opt => opt.MapFrom(src => src.DishesServed))
                .ForMember(dest => dest.ComboDetailsForChef, opt => opt.MapFrom(src => src.Combo.ComboDetails
                    .GroupBy(cd => cd.ComboId)
                    .Select(g => g.First())
                    .Select(cd => new ComboDetailForChefDTO
                    {
                        ComboName = cd.Combo.NameCombo,
                        ItemsInCombo = cd.Combo.ComboDetails.Select(item => new ItemInComboDTO
                        {
                            ItemName = item.Dish.ItemName,
                            QuantityDish = item.QuantityDish
                        }).ToList(),
                        Note = cd.Combo.Note,
                        OrderTime = src.OrderTime
                    }).ToList()));

            CreateMap<Setting, SettingAllDTO>()
            .ForMember(dest => dest.OpenTime, opt => opt.MapFrom(src => src.OpenTime.HasValue ? src.OpenTime.Value.ToString(@"hh\:mm\:ss") : null))
            .ForMember(dest => dest.CloseTime, opt => opt.MapFrom(src => src.CloseTime.HasValue ? src.CloseTime.Value.ToString(@"hh\:mm\:ss") : null));

            CreateMap<SettingAllDTO, Setting>()
                .ForMember(dest => dest.OpenTime, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.OpenTime) ? TimeSpan.Parse(src.OpenTime) : (TimeSpan?)null))
                .ForMember(dest => dest.CloseTime, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.CloseTime) ? TimeSpan.Parse(src.CloseTime) : (TimeSpan?)null));


            CreateMap<UpdateStatusTableByReservation, Table>()
                  .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.TableStatus));
            CreateMap<UpdateStatusTableByReservation, Table>()
                  .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.TableStatus));


            // Update  orders based on TableID


            //Create Order for Table
            CreateMap<CreateOrderForTableDTO, Order>();

            CreateMap<Dish, CreateOrderDetailDTO>()
         .ForMember(dest => dest.DiscountedPrice, opt => opt.MapFrom(
             src => src.DiscountId == null ? (decimal?)null :
                    src.DiscountId == 0 ? src.Price :
                    src.Price.HasValue && src.Discount != null ? src.Price.Value - (src.Price.Value * src.Discount.DiscountPercent / 100) : (decimal?)null
         ));

            CreateMap<Discount, DiscountAllDTO>().ReverseMap();
            CreateMap<CreateDiscount, Discount>();
            CreateMap<Discount, CreateDiscountResponse>();
            CreateMap<Discount, CreateDiscount>();

            CreateMap<Discount, DiscountDTO>();

            // Cancel Order
            CreateMap<CancelOrderTableDTO, Order>()
               .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

            //Get OrderDetail
            CreateMap<OrderDetail, GetOrderDetailDTO>()
          .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.Dish != null ? src.Dish.ItemName : null))
          .ForMember(dest => dest.NameCombo, opt => opt.MapFrom(src => src.Combo != null ? src.Combo.NameCombo : null))
          .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Dish != null ? src.Dish.ImageUrl : (src.Combo != null ? src.Combo.ImageUrl : null)))
          .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Dish != null ? src.Dish.Price : (src.Combo != null ? src.Combo.Price : (decimal?)null)));


            //Update Invoice
            CreateMap<UpdateInvoiceDTO, Invoice>()
              .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.CustomerName))
              .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
              .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address));

            CreateMap<Invoice, GetInvoiceByOrderDTO>();

            CreateMap<PrepaymentDTO, Invoice>()
            .ForMember(dest => dest.PaymentTime, opt => opt.MapFrom(src => src.PaymentTime))
            .ForMember(dest => dest.PaymentAmount, opt => opt.MapFrom(src => src.PaymentAmount))
            .ForMember(dest => dest.Taxcode, opt => opt.MapFrom(src => src.Taxcode))
            .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.PaymentStatus))

            .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.AccountId))
            .ForMember(dest => dest.AmountReceived, opt => opt.MapFrom(src => src.AmountReceived))
            .ForMember(dest => dest.ReturnAmount, opt => opt.MapFrom(src => src.ReturnAmount))
            .ForMember(dest => dest.PaymentMethods, opt => opt.MapFrom(src => src.PaymentMethods));

            CreateMap<Order, InvoiceOfSitting>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.PaymentTime, opt => opt.MapFrom(src => src.Invoice != null ? src.Invoice.PaymentTime : (DateTime?)null))
            .ForMember(dest => dest.PaymentAmount, opt => opt.MapFrom(src => src.Invoice != null ? src.Invoice.PaymentAmount : (decimal?)null))
            .ForMember(dest => dest.Taxcode, opt => opt.MapFrom(src => src.Invoice != null ? src.Invoice.Taxcode : (string?)null))
            .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.Invoice != null ? src.Invoice.PaymentStatus : 0))
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Invoice != null ? src.Invoice.CustomerName : (string?)null))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Invoice != null ? src.Invoice.Phone : (string?)null))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Invoice != null ? src.Invoice.Address : (string?)null))
            .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.Invoice != null ? src.Invoice.AccountId : (int?)null))
            .ForMember(dest => dest.AmountReceived, opt => opt.MapFrom(src => src.Invoice != null ? src.Invoice.AmountReceived : (decimal?)null))
            .ForMember(dest => dest.ReturnAmount, opt => opt.MapFrom(src => src.Invoice != null ? src.Invoice.ReturnAmount : (decimal?)null))
            .ForMember(dest => dest.PaymentMethods, opt => opt.MapFrom(src => src.Invoice != null ? src.Invoice.PaymentMethods : (int?)null))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Invoice != null ? src.Invoice.InvoiceLogs.Select(log => log.Description).FirstOrDefault() : (string?)null))
            .ForMember(dest => dest.TableStatus, opt => opt.MapFrom(src => src.OrderTables.FirstOrDefault().Table.Status));



            CreateMap<Order, UpdateAmountReceiving>()
        .ForMember(dest => dest.AmountReceived, opt => opt.MapFrom(src => src.Invoice.AmountReceived))
        .ForMember(dest => dest.Description, opt => opt.Ignore());


            CreateMap<Discount, GetDiscountByOrderID>();

            CreateMap<Discount, DiscountWithDishesDTO>()
               .ForMember(dest => dest.Dishes, opt => opt.MapFrom(src => src.Dishes));

            CreateMap<OrderDetail, OrderDetailForStaff>()
       .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.Dish.ItemName))
       .ForMember(dest => dest.ComboName, opt => opt.MapFrom(src => src.Combo.NameCombo))
       .ForMember(dest => dest.OrderType, opt => opt.MapFrom(src => src.Order.Type))
       .ForMember(dest => dest.OrderTime, opt => opt.MapFrom(src => src.Order.OrderDate))
       .ForMember(dest => dest.TableId, opt => opt.MapFrom(src => src.Order.OrderTables.FirstOrDefault().TableId))
       .ForMember(dest => dest.QuantityRequired, opt => opt.MapFrom(src => src.Quantity - src.DishesServed))
       // Map for TableLabel from Table entity
       .ForMember(dest => dest.TableLabel, opt => opt.MapFrom(src => src.Order.OrderTables.FirstOrDefault().Table.Lable))
       // Map for Floor from Table entity
       .ForMember(dest => dest.Floor, opt => opt.MapFrom(src => src.Order.OrderTables.FirstOrDefault().Table.Floor));


            CreateMap<Order, OrderDetailForStaffType1>()
                 .ForMember(dest => dest.OrderType, opt => opt.MapFrom(src => src.Type))
                 .ForMember(dest => dest.GuestAddress, opt => opt.MapFrom(src => src.Address.GuestAddress))
                 .ForMember(dest => dest.ConsigneeName, opt => opt.MapFrom(src => src.Address.ConsigneeName))
                 .ForMember(dest => dest.ItemInOrderDetails, opt => opt.MapFrom(src => src.OrderDetails))
                 .ForMember(dest => dest.Status, opt => opt.MapFrom<OrderStatusResolver>())
                  .ForMember(dest => dest.StatusOrder, opt => opt.MapFrom(src => src.Status))
                 .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount))
                   .ForMember(dest => dest.DiscountPercent, opt => opt.MapFrom(src => src.Discount != null ? src.Discount.DiscountPercent : (decimal?)null))
                 .ForMember(dest => dest.DiscountedPrice, opt => opt.Ignore())
                  .ForMember(dest => dest.CancelationReason, opt => opt.MapFrom(src => src.CancelationReason))
                   .ForMember(dest => dest.CancelBy, opt => opt.MapFrom(src => src.CancelBy))
                    .ForMember(dest => dest.CancelDate, opt => opt.MapFrom(src => src.CancelDate))
                     .ForMember(dest => dest.StaffId, opt => opt.MapFrom(src => src.StaffId))
                      .ForMember(dest => dest.ShipTime, opt => opt.MapFrom(src => src.ShipTime))
                       .ForMember(dest => dest.CollectedBy, opt => opt.MapFrom(src => src.CollectedBy))
                        .ForMember(dest => dest.AcceptBy, opt => opt.MapFrom(src => src.AcceptBy))
                         .ForMember(dest => dest.RefundDate, opt => opt.MapFrom(src => src.RefundDate));

            // Mapping OrderDetail to ItemInOrderDetail
            CreateMap<OrderDetail, ItemInOrderDetail>()
                .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.Dish != null ? src.Dish.ItemName : null))
                .ForMember(dest => dest.ComboName, opt => opt.MapFrom(src => src.Combo != null ? src.Combo.NameCombo : null))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.DishesServed, opt => opt.MapFrom(src => src.DishesServed))
                 .ForMember(dest => dest.ItemPrice, opt => opt.MapFrom(src => src.Dish.Price))
                 .ForMember(dest => dest.ComboPrice, opt => opt.MapFrom(src => src.Combo.Price))
                .ForMember(dest => dest.OrderTime, opt => opt.MapFrom(src => src.OrderTime));
               

            //Account
            CreateMap<Account, GetAccountDTO>();


			CreateMap<CreateAccountDTO, Account>()
			 .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive ?? false))
			 .ReverseMap();


			CreateMap<UpdateAccountDTO, Account>();


			CreateMap<AcceptOrderDTO, Invoice>()
		   .ForMember(dest => dest.PaymentTime, opt => opt.Ignore())
		   .ForMember(dest => dest.PaymentStatus, opt => opt.Ignore());

            CreateMap<Ingredient, IngredientSearchNameDTO>()
                .ForMember(dest => dest.MaterialName, opt => opt.MapFrom(src => src.Material.Name))
                .ForMember(dest => dest.Quantitative, opt => opt.MapFrom(src => src.Quantitative));

            CreateMap<Dish, DishSearchDTO>()
                .ForMember(dest => dest.Ingredients, opt => opt.MapFrom(src => src.Ingredients));
            CreateMap<Order, OrderAccountDTO>();

            CreateMap<Combo, ComboSearchDTO>()
                .ForMember(dest => dest.Dishes, opt => opt.MapFrom(src => src.ComboDetails.Select(cd => cd.Dish)));
            
            CreateMap<Account, GetAccountByRole>();

            CreateMap<Order, UpdateOrderAccountDTO>();

			CreateMap<News, NewsDTO>();

			CreateMap<UpdateProfileDTO, Account>();

			CreateMap<Reservation, GetReservationByOrderDTO>()
		  .ForMember(dest => dest.GuestAddress, opt => opt.MapFrom(src => src.Address.GuestAddress))
		  .ForMember(dest => dest.ConsigneeName, opt => opt.MapFrom(src => src.Address.ConsigneeName))
		  .ForMember(dest => dest.GuestPhone, opt => opt.MapFrom(src => src.Address.GuestPhone));



			CreateMap<Order, GetOrderCancelInfo>()
			   .ForMember(dest => dest.GuestAddress, opt => opt.MapFrom(src => src.Address.GuestAddress))
			   .ForMember(dest => dest.ConsigneeName, opt => opt.MapFrom(src => src.Address.ConsigneeName))
			   .ForMember(dest => dest.GuestPhone, opt => opt.MapFrom(src => src.Address.GuestPhone))
			   .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount))
               .ForMember(dest => dest.PaymentAmount, opt => opt.MapFrom(src => src.Invoice.PaymentAmount))
               .ForMember(dest => dest.Deposits, opt => opt.MapFrom(src => src.Deposits))
			   .ForMember(dest => dest.CancelationReason, opt => opt.MapFrom(src => src.CancelationReason))
			   .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate))
			   .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
			   .ForMember(dest => dest.RecevingOrder, opt => opt.MapFrom(src => src.RecevingOrder))
			   .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.AccountId))
			   .ForMember(dest => dest.InvoiceId, opt => opt.MapFrom(src => src.InvoiceId))
			   .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Note))
			   .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
			   .ForMember(dest => dest.DiscountId, opt => opt.MapFrom(src => src.DiscountId));

			CreateMap<Order, GetOrderUnpaidOfShipDTO>()

			   .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.Invoice.PaymentStatus))
			   .ForMember(dest => dest.TotalPaid, opt => opt.Ignore());

                  

            CreateMap<Order, GetInvoiceAndOrderInfo>()
    .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderId))
    .ForMember(dest => dest.InvoiceId, opt => opt.MapFrom(src => src.InvoiceId))
    .ForMember(dest => dest.PaymentTime, opt => opt.MapFrom(src => src.Invoice.PaymentTime))
    .ForMember(dest => dest.PaymentAmount, opt => opt.MapFrom(src => src.Invoice.PaymentAmount))
    .ForMember(dest => dest.Taxcode, opt => opt.MapFrom(src => src.Invoice.Taxcode))
    .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.Invoice.PaymentStatus))
    .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Invoice.CustomerName))
    .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Invoice.Phone))
    .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Invoice.Address))
    .ForMember(dest => dest.AmountReceived, opt => opt.MapFrom(src => src.Invoice.AmountReceived))
    .ForMember(dest => dest.ReturnAmount, opt => opt.MapFrom(src => src.Invoice.ReturnAmount))
    .ForMember(dest => dest.PaymentMethods, opt => opt.MapFrom(src => src.Invoice.PaymentMethods))
    .ReverseMap();


            CreateMap<Order, CategorySalesDTO>()
       .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.OrderDetails.First().Dish.CategoryId))
       .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.OrderDetails.First().Dish.Category.CategoryName))
       .ForMember(dest => dest.TotalSales, opt => opt.MapFrom(src => src.OrderDetails.Sum(od => od.Quantity ?? 0)));
           
            CreateMap<Order, ExportOrderDTO>()
                        .ForMember(dest => dest.Invoice, opt => opt.MapFrom(src => src.Invoice))
                        .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                        .ForMember(dest => dest.Guest, opt => opt.MapFrom(src => src.GuestPhoneNavigation))
                        .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.OrderDetails))
                        .ForMember(dest => dest.Reservations, opt => opt.MapFrom(src => src.Reservations));

            CreateMap<Invoice, ExportInvoiceDTO>();
            CreateMap<Address, ExportAddressDTO>();
            CreateMap<Guest, ExportGuestDTO>();

            CreateMap<OrderDetail, ExportOrderDetailDTO>()
                .ForMember(dest => dest.DishName, opt => opt.MapFrom(src => src.Dish.ItemName));

            CreateMap<Reservation, ExportReservationDTO>()
                .ForMember(dest => dest.TableIds, opt => opt.MapFrom(src => src.TableReservations.Select(tr => tr.TableId)));

            CreateMap<OrderTable, CreateOrderTable>().ReverseMap();
            CreateMap<FindTableByReservation, TableReservation>().ReverseMap();
            CreateMap<UpdateReservationOrderDTO, Reservation>();

            CreateMap<UpdateAmountInvoiceDTO, Invoice>().ReverseMap();
            CreateMap<Reservation, UpdateReservationStatusByOrder>().ReverseMap();
            CreateMap<Table, CreateTableDTO>().ReverseMap();
            CreateMap<TableReservation, TableReservationAllDTO>()
                  .ForMember(dest => dest.ReservationTime, opt => opt.MapFrom(src => src.Reservation.ReservationTime))
                .ReverseMap();
            CreateMap<UpdateDishQuantityDTO, Dish>()
           .ForMember(dest => dest.QuantityDish, opt => opt.MapFrom(src => src.QuantityDish));

            CreateMap<Order, OrderByID>()
             .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.OrderDetails))
             .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address));

            CreateMap<OrderDetail, OrderDetailDTO2>()
                .ForMember(dest => dest.DishName, opt => opt.MapFrom(src => src.Dish.ItemName))
                .ForMember(dest => dest.ComboName, opt => opt.MapFrom(src => src.Combo.NameCombo))
              .ForMember(dest => dest.DishName, opt => opt.MapFrom(src => src.Dish != null ? src.Dish.ItemName : null))
    .ForMember(dest => dest.ComboName, opt => opt.MapFrom(src => src.Combo != null ? src.Combo.NameCombo : null))
    .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Dish != null ? src.Dish.ImageUrl : src.Combo != null ? src.Combo.ImageUrl : null))
    .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Dish != null ? src.Dish.Price : src.Combo != null ? src.Combo.Price : (decimal?)null));

            CreateMap<Notification, NotificationAllDTO>().ReverseMap();
            CreateMap<NotificationCreateDTO, Notification>();
            CreateMap<Address, AddressDTO1>();

          
            CreateMap<OrderEmailDTO, Guest>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));
            CreateMap<Reservation, ReservationDTOByOrderId>();
            CreateMap<HashSet<Reservation>, IEnumerable<ReservationDTOByOrderId>>();
            CreateMap<StatiticsForManager, Order>().ReverseMap();
        }


        private static decimal? CalculateDiscountedPrice(OrderDetail src)
        {
            if (src.DishId != null && src.Dish != null)
            {
                if (src.Dish.DiscountId == null || src.Dish.DiscountId == 0)
                {
                    return src.Dish.Price;
                }

                if (src.Dish.Price.HasValue && src.Dish.Discount != null)
                {
                    return src.Dish.Price.Value - (src.Dish.Price.Value * src.Dish.Discount.DiscountPercent / 100);
                }
            }
            else if (src.ComboId != null && src.Combo != null)
            {
                return src.Combo.Price;
            }

            return null;
        }
        public class OrderStatusResolver : IValueResolver<Order, OrderDetailForStaffType1, int?>
        {
            public int? Resolve(Order source, OrderDetailForStaffType1 destination, int? destMember, ResolutionContext context)
            {
                var allDetails = source.OrderDetails;
                return allDetails.All(detail => detail.Quantity == detail.DishesServed) ? 1 : 0; // Assuming 1 for true and 0 for false
            }
        }


    }
}
