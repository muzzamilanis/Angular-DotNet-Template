using BusinessApi.Application.Services;
using BusinessApi.Domain.Interfaces;
using BusinessApi.Infrastructure.Repositories;

namespace BusinessApi.Api.Modules.ProductManagement
{
    public static class ProductModule
    {
        public static IServiceCollection AddProductModule(this IServiceCollection services)
        {
            // Register Product-specific services and repositories
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductService, ProductService>();
            
            return services;
        }
    }
}