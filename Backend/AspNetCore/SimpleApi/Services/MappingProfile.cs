using AutoMapper;
using SimpleApi.DTOs;
using SimpleApi.Models;

namespace SimpleApi.Services
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Company mappings
            CreateMap<Company, CompanyDto>();
            CreateMap<CreateCompanyDto, Company>();
            CreateMap<UpdateCompanyDto, Company>();

            // Customer mappings
            CreateMap<Customer, CustomerDto>()
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company!.Name));
            CreateMap<CreateCustomerDto, Customer>();
            CreateMap<UpdateCustomerDto, Customer>();

            // Product mappings
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company!.Name));
            CreateMap<CreateProductDto, Product>();
            CreateMap<UpdateProductDto, Product>();

            // Project mappings
            CreateMap<Project, ProjectDto>()
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company!.Name));
            CreateMap<CreateProjectDto, Project>();
            CreateMap<UpdateProjectDto, Project>();
            CreateMap<Project, ProjectListDto>();

            // SaleOrder mappings
            CreateMap<SaleOrder, SaleOrderDto>()
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project!.ProjectName))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company!.Name));
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