using AutoMapper;
using EHM_API.DTOs.CategoryDTO;
using EHM_API.DTOs.ComboDTO;
using EHM_API.DTOs.DishDTO;
using EHM_API.DTOs.HomeDTO;
using EHM_API.DTOs.OrderDTO;
using EHM_API.Models;

namespace EHM_API.Map
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<Dish, DishDTOAll>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.CategoryName : null))
                .ForMember(dest => dest.DiscountedPrice, opt => opt.MapFrom(src => src.Price.HasValue && src.Discount != null ? src.Price.Value - (src.Price.Value * src.Discount.DiscountAmount / 100) : src.Price))
                .ForMember(dest => dest.DiscountPercentage, opt => opt.MapFrom(src => src.Discount != null ? src.Discount.DiscountAmount : (int?)null));

            CreateMap<CreateDishDTO, Dish>();
            CreateMap<UpdateDishDTO, Dish>();

            CreateMap<Order, OrderDTOAll>()
                 .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Account != null ? src.Account.FirstName : null))
                 .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Account != null ? src.Account.LastName : null));


            CreateMap<CreateOrderDTO, Order>();
            CreateMap<UpdateOrderDTO, Order>();
            CreateMap<SearchOrdersRequestDTO, Order>();

            CreateMap<Category, CategoryDTO>().ReverseMap();
            CreateMap<CreateCategory, Category>().ReverseMap();
            CreateMap<Category, ViewCategoryDTO>().ReverseMap();

			CreateMap<Combo, ComboDTO>().ReverseMap();
			CreateMap<CreateComboDishDTO, Combo>().ReverseMap();

			CreateMap<CreateComboDTO, Combo>().ReverseMap();


			CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
        }
    }
}
