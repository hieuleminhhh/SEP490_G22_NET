﻿using AutoMapper;
using EHM_API.DTOs.CategoryDTO;
using EHM_API.DTOs.ComboDTO;
using EHM_API.DTOs.ComboDTO.EHM_API.DTOs.ComboDTO;
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
                 .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Account != null ? src.Account.FirstName : null))
                 .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Account != null ? src.Account.LastName : null))
                 .ForMember(dest => dest.GuestAddress, opt => opt.MapFrom(src => src.Address != null ? src.Address.GuestAddress : null))
                 .ForMember(dest => dest.ConsigneeName, opt => opt.MapFrom(src => src.Address != null ? src.Address.ConsigneeName : null));


            CreateMap<CreateOrderDTO, Order>();
            CreateMap<UpdateOrderDTO, Order>();
            CreateMap<SearchOrdersRequestDTO, Order>();

            CreateMap<Category, CategoryDTO>().ReverseMap();
            CreateMap<CreateCategory, Category>().ReverseMap();
            CreateMap<Category, ViewCategoryDTO>().ReverseMap();

		      CreateMap<Combo, ComboDTO>().ReverseMap();

			/*CreateMap<CreateComboDishDTO, Combo>()
				.ForMember(dest => dest., opt => opt.MapFrom(src => src.Dishes.Select(d => new ComboDetail { DishId = d.DishId })));*/

			CreateMap<ComboDetail, Dish>().ReverseMap();



			CreateMap<CreateComboDTO, Combo>().ReverseMap();


			CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
        }
    }
}
