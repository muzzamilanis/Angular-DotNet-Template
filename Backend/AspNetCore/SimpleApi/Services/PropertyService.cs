using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SimpleApi.Data;
using SimpleApi.DTOs;
using SimpleApi.Models;

namespace SimpleApi.Services
{
    public interface IPropertyService
    {
        Task<PagedResultDto<PropertyDto>> GetAllAsync(PagedResultRequestDto input, int companyId);
        Task<List<PropertyDto>> GetActivePropertiesAsync(int companyId);
        Task<PropertyDto> GetAsync(int id, int companyId);
        Task<PropertyDto> CreateAsync(CreatePropertyDto input, int companyId);
        Task<PropertyDto> UpdateAsync(UpdatePropertyDto input, int companyId);
        Task DeleteAsync(int id, int companyId);
    }

    public class PropertyService : IPropertyService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public PropertyService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedResultDto<PropertyDto>> GetAllAsync(PagedResultRequestDto input, int companyId)
        {
            var query = _context.Properties
                .Where(p => p.CompanyId == companyId)
                .Include(p => p.Company)
                .AsQueryable();
            
            var totalCount = await query.CountAsync();
            var properties = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .ToListAsync();
            
            var dtos = _mapper.Map<List<PropertyDto>>(properties);
            
            return new PagedResultDto<PropertyDto>(totalCount, dtos);
        }

        public async Task<List<PropertyDto>> GetActivePropertiesAsync(int companyId)
        {
            var properties = await _context.Properties
                .Include(p => p.Company)
                .Where(p => p.IsActive && p.CompanyId == companyId)
                .OrderBy(p => p.PropertyName)
                .ToListAsync();
            return _mapper.Map<List<PropertyDto>>(properties);
        }

        public async Task<PropertyDto> GetAsync(int id, int companyId)
        {
            var property = await _context.Properties
                .Where(p => p.Id == id && p.CompanyId == companyId)
                .FirstOrDefaultAsync();
            if (property == null)
            {
                throw new ArgumentException("Property not found");
            }
            return _mapper.Map<PropertyDto>(property);
        }

        public async Task<PropertyDto> CreateAsync(CreatePropertyDto input, int companyId)
        {
            var property = _mapper.Map<Property>(input);
            property.CompanyId = companyId;
            property.CreatedAt = DateTime.UtcNow;
            
            _context.Properties.Add(property);
            await _context.SaveChangesAsync();
            
            return _mapper.Map<PropertyDto>(property);
        }

        public async Task<PropertyDto> UpdateAsync(UpdatePropertyDto input, int companyId)
        {
            var property = await _context.Properties
                .Where(p => p.Id == input.Id && p.CompanyId == companyId)
                .FirstOrDefaultAsync();
            if (property == null)
            {
                throw new ArgumentException("Property not found");
            }
            
            _mapper.Map(input, property);
            property.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return _mapper.Map<PropertyDto>(property);
        }

        public async Task DeleteAsync(int id, int companyId)
        {
            var property = await _context.Properties
                .Where(p => p.Id == id && p.CompanyId == companyId)
                .FirstOrDefaultAsync();
            if (property == null)
            {
                throw new ArgumentException("Property not found");
            }
            
            property.IsActive = false;
            await _context.SaveChangesAsync();
        }
    }
}
