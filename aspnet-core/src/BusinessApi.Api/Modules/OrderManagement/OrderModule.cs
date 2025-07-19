using BusinessApi.Application.Services;
using BusinessApi.Domain.Interfaces;
using BusinessApi.Infrastructure.Repositories;

namespace BusinessApi.Api.Modules.OrderManagement
{
    public static class OrderModule
    {
        public static IServiceCollection AddOrderModule(this IServiceCollection services)
        {
            // Register Order-specific services and repositories
            services.AddScoped<ISaleOrderRepository, SaleOrderRepository>();
            services.AddScoped<ISaleOrderService, SaleOrderService>();
            
            return services;
        }
    }
}