using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SimpleApi.Data;
using SimpleApi.DTOs;
using SimpleApi.Models;

namespace SimpleApi.Services
{
    public interface IVendorService
    {
        Task<PagedResultDto<VendorDto>> GetAllAsync(PagedResultRequestDto input, int companyId);
        Task<List<VendorDto>> GetActiveVendorsAsync(int companyId);
        Task<VendorDto> GetAsync(int id, int companyId);
        Task<VendorDto> CreateAsync(CreateVendorDto input, int companyId);
        Task<VendorDto> UpdateAsync(UpdateVendorDto input, int companyId);
        Task DeleteAsync(int id, int companyId);
    }

    public class VendorService : IVendorService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public VendorService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedResultDto<VendorDto>> GetAllAsync(PagedResultRequestDto input, int companyId)
        {
            var query = _context.Vendors
                .Where(v => v.CompanyId == companyId)
                .Include(v => v.Company)
                .AsQueryable();
            
            var totalCount = await query.CountAsync();
            var vendors = await query.Skip(input.SkipCount).Take(input.MaxResultCount).ToListAsync();
            
            var dtos = _mapper.Map<List<VendorDto>>(vendors);
            
            return new PagedResultDto<VendorDto>(totalCount, dtos);
        }

        public async Task<List<VendorDto>> GetActiveVendorsAsync(int companyId)
        {
            var vendors = await _context.Vendors
                .Include(v => v.Company)
                .Where(v => v.IsActive && v.CompanyId == companyId)
                .ToListAsync();
            return _mapper.Map<List<VendorDto>>(vendors);
        }

        public async Task<VendorDto> GetAsync(int id, int companyId)
        {
            var vendor = await _context.Vendors
                .Where(v => v.Id == id && v.CompanyId == companyId)
                .FirstOrDefaultAsync();
            if (vendor == null)
            {
                throw new ArgumentException("Vendor not found");
            }
            return _mapper.Map<VendorDto>(vendor);
        }

        public async Task<VendorDto> CreateAsync(CreateVendorDto input, int companyId)
        {
            var vendor = _mapper.Map<Vendor>(input);
            vendor.CompanyId = companyId;
            vendor.CreatedAt = DateTime.UtcNow;
            _context.Vendors.Add(vendor);
            await _context.SaveChangesAsync();
            return _mapper.Map<VendorDto>(vendor);
        }

        public async Task<VendorDto> UpdateAsync(UpdateVendorDto input, int companyId)
        {
            var vendor = await _context.Vendors
                .Where(v => v.Id == input.Id && v.CompanyId == companyId)
                .FirstOrDefaultAsync();
            if (vendor == null)
            {
                throw new ArgumentException("Vendor not found");
            }
            
            _mapper.Map(input, vendor);
            vendor.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return _mapper.Map<VendorDto>(vendor);
        }

        public async Task DeleteAsync(int id, int companyId)
        {
            var vendor = await _context.Vendors
                .Where(v => v.Id == id && v.CompanyId == companyId)
                .FirstOrDefaultAsync();
            if (vendor == null)
            {
                throw new ArgumentException("Vendor not found");
            }
            
            _context.Vendors.Remove(vendor);
            await _context.SaveChangesAsync();
        }
    }
}
