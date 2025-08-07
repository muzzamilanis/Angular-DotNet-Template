using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SimpleApi.Data;
using SimpleApi.DTOs;
using SimpleApi.DTOs.CRM;
using SimpleApi.Models.CRM;

namespace SimpleApi.Services.CRM
{
    public interface ICrmFormTemplateService
    {
        Task<PagedResultDto<CrmFormTemplateDto>> GetAllAsync(PagedResultRequestDto input, int companyId);
        Task<List<CrmFormTemplateDto>> GetActiveTemplatesAsync(int companyId);
        Task<List<CrmFormTemplateDto>> GetTemplatesByBusinessNatureAsync(int businessNatureId, int companyId);
        Task<CrmFormTemplateDto> GetAsync(int id, int companyId);
        Task<CrmFormTemplateDto> CreateAsync(CreateCrmFormTemplateDto input, int companyId);
        Task<CrmFormTemplateDto> UpdateAsync(UpdateCrmFormTemplateDto input, int companyId);
        Task DeleteAsync(int id, int companyId);
    }

    public class CrmFormTemplateService : ICrmFormTemplateService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CrmFormTemplateService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedResultDto<CrmFormTemplateDto>> GetAllAsync(PagedResultRequestDto input, int companyId)
        {
            var query = _context.CrmFormTemplates
                .Where(t => t.CompanyId == companyId)
                .Include(t => t.BusinessNature)
                .Include(t => t.Company)
                .AsQueryable();

            if (!string.IsNullOrEmpty(input.Filter))
            {
                query = query.Where(t => t.Name.Contains(input.Filter) || 
                                   (t.Description != null && t.Description.Contains(input.Filter)));
            }

            var totalCount = await query.CountAsync();
            var templates = await query.Skip(input.SkipCount).Take(input.MaxResultCount).ToListAsync();

            var dtos = _mapper.Map<List<CrmFormTemplateDto>>(templates);

            return new PagedResultDto<CrmFormTemplateDto>(totalCount, dtos);
        }

        public async Task<List<CrmFormTemplateDto>> GetActiveTemplatesAsync(int companyId)
        {
            var templates = await _context.CrmFormTemplates
                .Include(t => t.BusinessNature)
                .Include(t => t.Company)
                .Where(t => t.IsActive && t.CompanyId == companyId)
                .ToListAsync();
            return _mapper.Map<List<CrmFormTemplateDto>>(templates);
        }

        public async Task<List<CrmFormTemplateDto>> GetTemplatesByBusinessNatureAsync(int businessNatureId, int companyId)
        {
            var templates = await _context.CrmFormTemplates
                .Include(t => t.BusinessNature)
                .Include(t => t.Company)
                .Where(t => t.IsActive && t.CompanyId == companyId && t.BusinessNatureId == businessNatureId)
                .ToListAsync();
            return _mapper.Map<List<CrmFormTemplateDto>>(templates);
        }

        public async Task<CrmFormTemplateDto> GetAsync(int id, int companyId)
        {
            var template = await _context.CrmFormTemplates
                .Include(t => t.BusinessNature)
                .Include(t => t.Company)
                .Where(t => t.Id == id && t.CompanyId == companyId)
                .FirstOrDefaultAsync();
            if (template == null)
            {
                throw new ArgumentException("Form template not found");
            }
            return _mapper.Map<CrmFormTemplateDto>(template);
        }

        public async Task<CrmFormTemplateDto> CreateAsync(CreateCrmFormTemplateDto input, int companyId)
        {
            var template = _mapper.Map<CrmFormTemplate>(input);
            template.CompanyId = companyId;
            template.CreatedAt = DateTime.UtcNow;
            template.UpdatedAt = DateTime.UtcNow;
            
            _context.CrmFormTemplates.Add(template);
            await _context.SaveChangesAsync();
            
            // Reload with includes
            await _context.Entry(template)
                .Reference(t => t.BusinessNature)
                .LoadAsync();
            await _context.Entry(template)
                .Reference(t => t.Company)
                .LoadAsync();
                
            return _mapper.Map<CrmFormTemplateDto>(template);
        }

        public async Task<CrmFormTemplateDto> UpdateAsync(UpdateCrmFormTemplateDto input, int companyId)
        {
            var template = await _context.CrmFormTemplates
                .Include(t => t.BusinessNature)
                .Include(t => t.Company)
                .Where(t => t.Id == input.Id && t.CompanyId == companyId)
                .FirstOrDefaultAsync();
            if (template == null)
            {
                throw new ArgumentException("Form template not found");
            }

            _mapper.Map(input, template);
            template.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return _mapper.Map<CrmFormTemplateDto>(template);
        }

        public async Task DeleteAsync(int id, int companyId)
        {
            var template = await _context.CrmFormTemplates
                .Where(t => t.Id == id && t.CompanyId == companyId)
                .FirstOrDefaultAsync();
            if (template == null)
            {
                throw new ArgumentException("Form template not found");
            }

            _context.CrmFormTemplates.Remove(template);
            await _context.SaveChangesAsync();
        }
    }
}