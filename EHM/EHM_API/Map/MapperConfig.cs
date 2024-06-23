using AutoMapper;
using EHM_API.DTOs.CartDTO.Guest;
using EHM_API.DTOs.CategoryDTO;
using EHM_API.DTOs.CategoryDTO.Guest;
using EHM_API.DTOs.CategoryDTO.Manager;
using EHM_API.DTOs.ComboDTO.EHM_API.DTOs.ComboDTO;
using EHM_API.DTOs.ComboDTO.Guest;
using EHM_API.DTOs.ComboDTO.Manager;
using EHM_API.DTOs.DishDTO.Manager;
using EHM_API.DTOs.GuestDTO.Guest;
using EHM_API.DTOs.HomeDTO;
using EHM_API.DTOs.IngredientDTO.Manager;
using EHM_API.DTOs.MaterialDTO;
using EHM_API.DTOs.OrderDTO.Guest;
using EHM_API.DTOs.OrderDTO.Manager;
using EHM_API.DTOs.ReservationDTO.Guest;
using EHM_API.DTOs.TableDTO;
using EHM_API.Models;

namespace EHM_API.Map
{
	public class MapperConfig : Profile
	{
		public MapperConfig()
		{
			CreateMap<Dish, DishDTOAll>()
				.ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.CategoryName : null))
				.ForMember(dest => dest.DiscountedPrice, opt => opt.MapFrom(
					src => src.DiscountId == null ? (decimal?)null :
						   src.DiscountId == 0 ? src.Price :
						   src.Price.HasValue && src.Discount != null ? src.Price.Value - (src.Price.Value * src.Discount.DiscountAmount / 100) : (decimal?)null
					))
				.ForMember(dest => dest.DiscountPercentage, opt => opt.MapFrom(
					src => src.DiscountId == null || src.DiscountId == 0 ? (int?)null : src.Discount != null ? src.Discount.DiscountAmount : (int?)null
					));

			CreateMap<CreateDishDTO, Dish>();
			CreateMap<UpdateDishDTO, Dish>();

			CreateMap<Order, OrderDTOAll>()
				 .ForMember(dest => dest.GuestAddress, opt => opt.MapFrom(src => src.Address != null ? src.Address.GuestAddress : null))
				 .ForMember(dest => dest.ConsigneeName, opt => opt.MapFrom(src => src.Address != null ? src.Address.ConsigneeName : null));


			CreateMap<CreateOrderDTO, Order>();
			CreateMap<UpdateOrderDTO, Order>();
			CreateMap<SearchOrdersRequestDTO, Order>();

			//Check order guestphone
			CreateMap<OrderDetail, OrderDetailDTO>()
			.ForMember(dest => dest.NameCombo, opt => opt.MapFrom(src => src.Combo != null ? src.Combo.NameCombo : null))
			.ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.Dish != null ? src.Dish.ItemName : null))
			.ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Combo != null ? src.Combo.ImageUrl : (src.Dish != null ? src.Dish.ImageUrl : null)))
			.ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Combo != null && src.Combo.Price.HasValue ? src.Combo.Price : (src.Dish != null ? src.Dish.Price : (decimal?)null)))
			.ForMember(dest => dest.DiscountedPrice, opt => opt.MapFrom(src => src.Dish != null && src.Dish.DiscountId != null ? (src.Dish.DiscountId == 0 ? src.Dish.Price : (src.Dish.Price.HasValue && src.Dish.Discount != null ? src.Dish.Price.Value - (src.Dish.Price.Value * src.Dish.Discount.DiscountAmount / 100) : (decimal?)null)) : (decimal?)null));

			CreateMap<Order, SearchPhoneOrderDTO>()
				.ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.OrderDetails))
				.ForMember(dest => dest.GuestAddress, opt => opt.MapFrom(src => src.Address.GuestAddress))
				.ForMember(dest => dest.ConsigneeName, opt => opt.MapFrom(src => src.Address.ConsigneeName))
				.ForMember(dest => dest.PaymentMethods, opt => opt.MapFrom(src =>
					src.Deposits == 0 ? 1 :
					src.Deposits == src.TotalAmount / 2 ? 2 :
					src.Deposits == src.TotalAmount ? 3 : 0))
				.ReverseMap();



			CreateMap<Dish, OrderDetailsDTO>()
				.ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
				.ForMember(dest => dest.DiscountedPrice, opt => opt.MapFrom(
					src => src.DiscountId == null ? (decimal?)null :
						src.DiscountId == 0 ? src.Price :
						src.Price.HasValue && src.Discount != null ? src.Price.Value - (src.Price.Value * src.Discount.DiscountAmount / 100) : (decimal?)null
				))
				.ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.Price));

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


			// Reservation to ReservationRequestDTO
			// Reservation to ReservationDetailDTO
			CreateMap<Reservation, ReservationDetailDTO>()
				.ForMember(dest => dest.ConsigneeName,
						   opt => opt.MapFrom(src => src.Order != null ? src.Order.Address.ConsigneeName : null))
				.ForMember(dest => dest.GuestPhone,
						   opt => opt.MapFrom(src => src.GuestPhone))
				.ForMember(dest => dest.ReservationTime,
						   opt => opt.MapFrom(src => src.ReservationTime))
				.ForMember(dest => dest.Status,
						   opt => opt.MapFrom(src => src.Status))
				.ForMember(dest => dest.TableId,
						   opt => opt.MapFrom(src => src.TableId))
				.ForMember(dest => dest.GuestNumber,
						   opt => opt.MapFrom(src => src.GuestNumber))
				.ForMember(dest => dest.Note,
						   opt => opt.MapFrom(src => src.Note))
				.ForMember(dest => dest.Order,
						   opt => opt.MapFrom(src => src.Order));

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
					return src.Dish.Price.Value - (src.Dish.Price.Value * src.Dish.Discount.DiscountAmount / 100);
				}
			}
			else if (src.ComboId != null && src.Combo != null)
			{
				return src.Combo.Price;
			}

			return null;
		}





	}
}
