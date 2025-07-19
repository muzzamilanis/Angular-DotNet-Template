using Microsoft.EntityFrameworkCore;
using BusinessApi.Infrastructure.Data;
using BusinessApi.Application.Services;
using BusinessApi.Application.Mappings;

namespace BusinessApi.Api.Modules.Shared
{
    public static class SharedModule
    {
        public static IServiceCollection AddSharedModule(this IServiceCollection services, IConfiguration configuration)
        {
            // Add Entity Framework with SQLite
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("DefaultConnection") ?? 
                    "Data Source=BusinessApi.db"));

            // Add AutoMapper
            services.AddAutoMapper(typeof(MappingProfile));

            // Add shared services
            services.AddScoped<IAuthService, AuthService>();
            
            return services;
        }
    }
}