using AutoMapper;
using BusinessApi.Application.DTOs;
using BusinessApi.Domain.Entities;

namespace BusinessApi.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Customer mappings
            CreateMap<Customer, CustomerDto>();
            CreateMap<CreateCustomerDto, Customer>();
            CreateMap<UpdateCustomerDto, Customer>();

            // Product mappings
            CreateMap<Product, ProductDto>();
            CreateMap<CreateProductDto, Product>();
            CreateMap<UpdateProductDto, Product>();

            // SaleOrder mappings
            CreateMap<SaleOrder, SaleOrderDto>();
            CreateMap<CreateSaleOrderDto, SaleOrder>()
                .ForMember(dest => dest.Items, opt => opt.Ignore()); // Handle items manually
            CreateMap<UpdateSaleOrderDto, SaleOrder>()
                .ForMember(dest => dest.Items, opt => opt.Ignore()); // Handle items manually

            // SaleOrderItem mappings
            CreateMap<SaleOrderItem, SaleOrderItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product!.Name));
            CreateMap<CreateSaleOrderItemDto, SaleOrderItem>();
        }
    }
}