using AutoMapper;
using SimpleApi.DTOs;
using SimpleApi.DTOs.CRM;
using SimpleApi.Models;
using SimpleApi.Models.CRM;

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
                .ForMember(dest => dest.ServiceTypeName, opt => opt.MapFrom(src => src.ServiceType!.Name))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company!.Name));
            CreateMap<CreateSaleOrderDto, SaleOrder>()
                .ForMember(dest => dest.Items, opt => opt.Ignore()); // Handle items manually
            CreateMap<UpdateSaleOrderDto, SaleOrder>()
                .ForMember(dest => dest.Items, opt => opt.Ignore()); // Handle items manually

            // SaleOrderItem mappings
            CreateMap<SaleOrderItem, SaleOrderItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product!.Name));
            CreateMap<CreateSaleOrderItemDto, SaleOrderItem>();

            // ServiceType mappings
            CreateMap<ServiceType, ServiceTypeDto>()
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company!.Name));
            CreateMap<CreateServiceTypeDto, ServiceType>();

            // CRM mappings
            ConfigureCrmMappings();
        }

        private void ConfigureCrmMappings()
        {
            // Lead mappings
            CreateMap<Lead, LeadDto>()
                .ForMember(dest => dest.BusinessNatureName, opt => opt.MapFrom(src => src.BusinessNature!.DisplayName))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company!.Name));
            CreateMap<CreateLeadDto, Lead>();
            CreateMap<UpdateLeadDto, Lead>();

            // Contact mappings
            CreateMap<Contact, ContactDto>()
                .ForMember(dest => dest.LeadName, opt => opt.MapFrom(src => src.Lead!.FullName))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company!.Name));
            CreateMap<CreateContactDto, Contact>();
            CreateMap<UpdateContactDto, Contact>();

            // Deal mappings
            CreateMap<Deal, DealDto>()
                .ForMember(dest => dest.LeadName, opt => opt.MapFrom(src => src.Lead!.FullName))
                .ForMember(dest => dest.ContactName, opt => opt.MapFrom(src => src.Contact != null ? $"{src.Contact.FirstName} {src.Contact.LastName}" : null))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company!.Name));
            CreateMap<CreateDealDto, Deal>();
            CreateMap<UpdateDealDto, Deal>();

            // CrmTask mappings
            CreateMap<CrmTask, CrmTaskDto>()
                .ForMember(dest => dest.AssignedToUserName, opt => opt.MapFrom(src => src.AssignedToUser!.Name))
                .ForMember(dest => dest.LeadName, opt => opt.MapFrom(src => src.Lead!.FullName))
                .ForMember(dest => dest.ContactName, opt => opt.MapFrom(src => src.Contact != null ? $"{src.Contact.FirstName} {src.Contact.LastName}" : null))
                .ForMember(dest => dest.DealTitle, opt => opt.MapFrom(src => src.Deal!.Title))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company!.Name));
            CreateMap<CreateCrmTaskDto, CrmTask>();
            CreateMap<UpdateCrmTaskDto, CrmTask>();

            // CrmFormTemplate mappings
            CreateMap<CrmFormTemplate, CrmFormTemplateDto>()
                .ForMember(dest => dest.BusinessNatureName, opt => opt.MapFrom(src => src.BusinessNature!.DisplayName))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company!.Name));
            CreateMap<CreateCrmFormTemplateDto, CrmFormTemplate>();
            CreateMap<UpdateCrmFormTemplateDto, CrmFormTemplate>();

            // CrmFormSubmission mappings
            CreateMap<CrmFormSubmission, CrmFormSubmissionDto>()
                .ForMember(dest => dest.FormTemplateName, opt => opt.MapFrom(src => src.FormTemplate!.Name))
                .ForMember(dest => dest.LeadName, opt => opt.MapFrom(src => src.Lead!.FullName))
                .ForMember(dest => dest.ContactName, opt => opt.MapFrom(src => src.Contact != null ? $"{src.Contact.FirstName} {src.Contact.LastName}" : null))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company!.Name));
            CreateMap<CreateCrmFormSubmissionDto, CrmFormSubmission>();
            CreateMap<UpdateCrmFormSubmissionDto, CrmFormSubmission>();
        }
    }
}