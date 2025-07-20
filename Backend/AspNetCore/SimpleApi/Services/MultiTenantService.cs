using Microsoft.EntityFrameworkCore;
using SimpleApi.Data;
using SimpleApi.DTOs;
using SimpleApi.Models;

namespace SimpleApi.Services
{
    public interface IBusinessNatureService
    {
        Task<List<BusinessNatureDto>> GetAllAsync();
        Task<BusinessNatureDto?> GetByIdAsync(int id);
        Task<IndustryConfigDto?> GetIndustryConfigAsync(int businessNatureId);
    }

    public interface ICompanyService
    {
        Task<CompanyProfileDto?> GetByTenantIdAsync(int tenantId);
        Task<CompanyProfileDto> CreateAsync(CreateCompanyProfileDto dto, int tenantId);
        Task<CompanyProfileDto?> UpdateAsync(int tenantId, CreateCompanyProfileDto dto);
    }

    public class BusinessNatureService : IBusinessNatureService
    {
        private readonly ApplicationDbContext _context;

        public BusinessNatureService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<BusinessNatureDto>> GetAllAsync()
        {
            return await _context.BusinessNatures
                .Where(bn => bn.IsActive)
                .Select(bn => new BusinessNatureDto
                {
                    Id = bn.Id,
                    Name = bn.Name,
                    DisplayName = bn.DisplayName,
                    Description = bn.Description,
                    ConfigSchema = bn.ConfigSchema,
                    IsActive = bn.IsActive,
                    CreatedDate = bn.CreatedDate
                })
                .ToListAsync();
        }

        public async Task<BusinessNatureDto?> GetByIdAsync(int id)
        {
            return await _context.BusinessNatures
                .Where(bn => bn.Id == id && bn.IsActive)
                .Select(bn => new BusinessNatureDto
                {
                    Id = bn.Id,
                    Name = bn.Name,
                    DisplayName = bn.DisplayName,
                    Description = bn.Description,
                    ConfigSchema = bn.ConfigSchema,
                    IsActive = bn.IsActive,
                    CreatedDate = bn.CreatedDate
                })
                .FirstOrDefaultAsync();
        }

        public async Task<IndustryConfigDto?> GetIndustryConfigAsync(int businessNatureId)
        {
            var businessNature = await _context.BusinessNatures
                .FirstOrDefaultAsync(bn => bn.Id == businessNatureId && bn.IsActive);

            if (businessNature == null)
                return null;

            var modules = await _context.IndustryModules
                .Where(im => im.BusinessNatureId == businessNatureId && im.IsEnabled)
                .Select(im => new IndustryModuleDto
                {
                    Id = im.Id,
                    BusinessNatureId = im.BusinessNatureId,
                    ModuleName = im.ModuleName,
                    DisplayName = im.DisplayName,
                    ModuleConfig = im.ModuleConfig,
                    IsEnabled = im.IsEnabled,
                    CreatedDate = im.CreatedDate
                })
                .ToListAsync();

            var formConfigs = await _context.IndustryFormConfigs
                .Where(ifc => ifc.BusinessNatureId == businessNatureId && ifc.IsActive)
                .Select(ifc => new IndustryFormConfigDto
                {
                    Id = ifc.Id,
                    BusinessNatureId = ifc.BusinessNatureId,
                    FormType = ifc.FormType,
                    FieldSchema = ifc.FieldSchema,
                    ValidationRules = ifc.ValidationRules,
                    IsActive = ifc.IsActive,
                    CreatedDate = ifc.CreatedDate
                })
                .ToListAsync();

            return new IndustryConfigDto
            {
                BusinessNatureId = businessNatureId,
                BusinessNature = new BusinessNatureDto
                {
                    Id = businessNature.Id,
                    Name = businessNature.Name,
                    DisplayName = businessNature.DisplayName,
                    Description = businessNature.Description,
                    ConfigSchema = businessNature.ConfigSchema,
                    IsActive = businessNature.IsActive,
                    CreatedDate = businessNature.CreatedDate
                },
                Modules = modules,
                FormConfigs = formConfigs
            };
        }
    }

    public class CompanyService : ICompanyService
    {
        private readonly ApplicationDbContext _context;

        public CompanyService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CompanyProfileDto?> GetByTenantIdAsync(int tenantId)
        {
            return await _context.CompanyProfiles
                .Include(cp => cp.BusinessNature)
                .Where(cp => cp.TenantId == tenantId && cp.IsActive)
                .Select(cp => new CompanyProfileDto
                {
                    Id = cp.Id,
                    TenantId = cp.TenantId,
                    BusinessNatureId = cp.BusinessNatureId,
                    CompanyName = cp.CompanyName,
                    Address = cp.Address,
                    Phone = cp.Phone,
                    Email = cp.Email,
                    CustomFields = cp.CustomFields,
                    IsActive = cp.IsActive,
                    CreatedDate = cp.CreatedDate,
                    BusinessNature = cp.BusinessNature != null ? new BusinessNatureDto
                    {
                        Id = cp.BusinessNature.Id,
                        Name = cp.BusinessNature.Name,
                        DisplayName = cp.BusinessNature.DisplayName,
                        Description = cp.BusinessNature.Description,
                        ConfigSchema = cp.BusinessNature.ConfigSchema,
                        IsActive = cp.BusinessNature.IsActive,
                        CreatedDate = cp.BusinessNature.CreatedDate
                    } : null
                })
                .FirstOrDefaultAsync();
        }

        public async Task<CompanyProfileDto> CreateAsync(CreateCompanyProfileDto dto, int tenantId)
        {
            var companyProfile = new CompanyProfile
            {
                TenantId = tenantId,
                BusinessNatureId = dto.BusinessNatureId,
                CompanyName = dto.CompanyName,
                Address = dto.Address,
                Phone = dto.Phone,
                Email = dto.Email,
                CustomFields = dto.CustomFields,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            _context.CompanyProfiles.Add(companyProfile);
            await _context.SaveChangesAsync();

            return await GetByTenantIdAsync(tenantId) ?? throw new InvalidOperationException("Failed to create company profile");
        }

        public async Task<CompanyProfileDto?> UpdateAsync(int tenantId, CreateCompanyProfileDto dto)
        {
            var companyProfile = await _context.CompanyProfiles
                .FirstOrDefaultAsync(cp => cp.TenantId == tenantId && cp.IsActive);

            if (companyProfile == null)
                return null;

            companyProfile.BusinessNatureId = dto.BusinessNatureId;
            companyProfile.CompanyName = dto.CompanyName;
            companyProfile.Address = dto.Address;
            companyProfile.Phone = dto.Phone;
            companyProfile.Email = dto.Email;
            companyProfile.CustomFields = dto.CustomFields;

            await _context.SaveChangesAsync();

            return await GetByTenantIdAsync(tenantId);
        }
    }
}