using BusinessApi.Application.Services;
using BusinessApi.Domain.Interfaces;
using BusinessApi.Infrastructure.Repositories;

namespace BusinessApi.Api.Modules.CustomerManagement
{
    public static class CustomerModule
    {
        public static IServiceCollection AddCustomerModule(this IServiceCollection services)
        {
            // Register Customer-specific services and repositories
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ICustomerService, CustomerService>();
            
            return services;
        }
    }
}