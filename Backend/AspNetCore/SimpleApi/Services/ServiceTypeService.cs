using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SimpleApi.Data;
using SimpleApi.DTOs;
using SimpleApi.Models;

namespace SimpleApi.Services
{
    public interface IServiceTypeService
    {
        Task<PagedResultDto<ServiceTypeDto>> GetServiceTypesAsync(PagedResultRequestDto input, int companyId);
        Task<ServiceTypeDto> GetServiceTypeAsync(int id);
        Task<ServiceTypeDto> CreateServiceTypeAsync(CreateServiceTypeDto input);
        Task<ServiceTypeDto> UpdateServiceTypeAsync(int id, CreateServiceTypeDto input);
        Task DeleteServiceTypeAsync(int id);
    }

    public class ServiceTypeService : IServiceTypeService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ServiceTypeService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedResultDto<ServiceTypeDto>> GetServiceTypesAsync(PagedResultRequestDto input, int companyId)
        {
            var query = _context.ServiceTypes
                .Where(x => x.IsActive && x.CompanyId == companyId)
                .AsQueryable();

            if (!string.IsNullOrEmpty(input.Filter))
            {
                query = query.Where(x => x.Name.Contains(input.Filter));
            }

            if (!string.IsNullOrEmpty(input.Sorting))
            {
                // Simple sorting implementation
                if (input.Sorting.ToLower().Contains("name"))
                {
                    query = input.Sorting.ToLower().Contains("desc")
                        ? query.OrderByDescending(x => x.Name)
                        : query.OrderBy(x => x.Name);
                }
            }
            else
            {
                query = query.OrderBy(x => x.Name);
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .Include(x => x.Company)
                .Select(x => new ServiceTypeDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    IsActive = x.IsActive,
                    CompanyId = x.CompanyId,
                    CompanyName = x.Company != null ? x.Company.Name : null
                })
                .ToListAsync();

            return new PagedResultDto<ServiceTypeDto>(totalCount, items);
        }

        public async Task<ServiceTypeDto> GetServiceTypeAsync(int id)
        {
            var serviceType = await _context.ServiceTypes
                .Include(x => x.Company)
                .FirstOrDefaultAsync(x => x.Id == id && x.IsActive);

            if (serviceType == null)
                throw new Exception("ServiceType not found");

            return new ServiceTypeDto
            {
                Id = serviceType.Id,
                Name = serviceType.Name,
                Description = serviceType.Description,
                IsActive = serviceType.IsActive,
                CompanyId = serviceType.CompanyId,
                CompanyName = serviceType.Company?.Name
            };
        }

        public async Task<ServiceTypeDto> CreateServiceTypeAsync(CreateServiceTypeDto input)
        {
            var serviceType = new ServiceType
            {
                Name = input.Name,
                Description = input.Description,
                IsActive = input.IsActive,
                CompanyId = input.CompanyId,
                CreatedAt = DateTime.UtcNow
            };

            _context.ServiceTypes.Add(serviceType);
            await _context.SaveChangesAsync();

            return await GetServiceTypeAsync(serviceType.Id);
        }

        public async Task<ServiceTypeDto> UpdateServiceTypeAsync(int id, CreateServiceTypeDto input)
        {
            var serviceType = await _context.ServiceTypes.FirstOrDefaultAsync(x => x.Id == id && x.IsActive);
            if (serviceType == null)
                throw new Exception("ServiceType not found");

            serviceType.Name = input.Name;
            serviceType.Description = input.Description;
            serviceType.IsActive = input.IsActive;
            serviceType.CompanyId = input.CompanyId;

			await _context.SaveChangesAsync();

            return await GetServiceTypeAsync(serviceType.Id);
        }

        public async Task DeleteServiceTypeAsync(int id)
        {
            var serviceType = await _context.ServiceTypes.FirstOrDefaultAsync(x => x.Id == id);
            if (serviceType == null)
                throw new Exception("ServiceType not found");

            serviceType.IsActive = false;
            await _context.SaveChangesAsync();
        }
    }
}