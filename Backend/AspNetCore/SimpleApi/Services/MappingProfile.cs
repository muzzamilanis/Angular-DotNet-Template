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

            // Vendor mappings
            CreateMap<Vendor, VendorDto>()
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company!.Name));
            CreateMap<CreateVendorDto, Vendor>();
            CreateMap<UpdateVendorDto, Vendor>();

            // Product mappings
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company!.Name));
            CreateMap<CreateProductDto, Product>();
            CreateMap<UpdateProductDto, Product>();

            // Project mappings
            CreateMap<Project, ProjectDto>()
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company!.Name))
                .ForMember(dest => dest.ServiceTypeName, opt => opt.MapFrom(src => src.ServiceType!.Name));
            CreateMap<CreateProjectDto, Project>();
            CreateMap<UpdateProjectDto, Project>();
            CreateMap<Project, ProjectListDto>()
                .ForMember(dest => dest.ServiceTypeName, opt => opt.MapFrom(src => src.ServiceType!.Name));

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

            // Currency mappings
            CreateMap<Currency, CurrencyDto>();
            CreateMap<CreateCurrencyDto, Currency>();
            CreateMap<UpdateCurrencyDto, Currency>();

            // Property mappings
            CreateMap<Property, PropertyDto>();
            CreateMap<CreatePropertyDto, Property>();
            CreateMap<UpdatePropertyDto, Property>();

            // PropertyPurchase mappings
            CreateMap<PropertyPurchase, PropertyPurchaseDto>()
                .ForMember(dest => dest.Property, opt => opt.MapFrom(src => src.Property));
            CreateMap<CreatePropertyPurchaseDto, PropertyPurchase>();
            CreateMap<UpdatePropertyPurchaseDto, PropertyPurchase>();

            // PropertySale mappings
            CreateMap<PropertySale, PropertySaleDto>()
                .ForMember(dest => dest.Property, opt => opt.MapFrom(src => src.Property));
            CreateMap<CreatePropertySaleDto, PropertySale>();
            CreateMap<UpdatePropertySaleDto, PropertySale>();

            // Account mappings
            CreateMap<Account, AccountDto>();
            CreateMap<CreateAccountDto, Account>();
            CreateMap<UpdateAccountDto, Account>();

            // Voucher mappings
            CreateMap<Voucher, VoucherDto>()
                .ForMember(dest => dest.JournalEntries, opt => opt.MapFrom(src => src.JournalEntries));
            CreateMap<CreateVoucherDto, Voucher>();

            // JournalEntry mappings
            CreateMap<JournalEntry, JournalEntryDto>()
                .ForMember(dest => dest.AccountName, opt => opt.MapFrom(src => src.Account!.AccountName));
            CreateMap<CreateJournalEntryDto, JournalEntry>();

            // Construction mappings
            CreateMap<ConstructionProject, ConstructionProjectDto>();
            CreateMap<CreateConstructionProjectDto, ConstructionProject>();
            CreateMap<UpdateConstructionProjectDto, ConstructionProject>();

            CreateMap<ProjectClient, ProjectClientDto>();
            CreateMap<CreateProjectClientDto, ProjectClient>();
            CreateMap<UpdateProjectClientDto, ProjectClient>();

            CreateMap<ProjectInvestor, ProjectInvestorDto>();
            CreateMap<CreateProjectInvestorDto, ProjectInvestor>();
            CreateMap<UpdateProjectInvestorDto, ProjectInvestor>();

            CreateMap<ProjectPayment, ProjectPaymentDto>();
            CreateMap<CreateProjectPaymentDto, ProjectPayment>();

            CreateMap<ProjectExpense, ProjectExpenseDto>();
            CreateMap<CreateProjectExpenseDto, ProjectExpense>();

            CreateMap<ClientLedgerEntry, ClientLedgerEntryDto>();
            CreateMap<InvestorLedgerEntry, InvestorLedgerEntryDto>();
            CreateMap<ProjectLedgerEntry, ProjectLedgerEntryDto>();
        }
    }
}