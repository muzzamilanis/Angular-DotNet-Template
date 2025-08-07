using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SimpleApi.Data;
using SimpleApi.DTOs;
using SimpleApi.DTOs.CRM;
using SimpleApi.Models.CRM;

namespace SimpleApi.Services.CRM
{
    public interface ILeadService
    {
        Task<PagedResultDto<LeadDto>> GetAllAsync(PagedResultRequestDto input, int companyId);
        Task<List<LeadDto>> GetActiveLeadsAsync(int companyId);
        Task<LeadDto> GetAsync(int id, int companyId);
        Task<LeadDto> CreateAsync(CreateLeadDto input, int companyId);
        Task<LeadDto> UpdateAsync(UpdateLeadDto input, int companyId);
        Task DeleteAsync(int id, int companyId);
    }

    public class LeadService : ILeadService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public LeadService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedResultDto<LeadDto>> GetAllAsync(PagedResultRequestDto input, int companyId)
        {
            var query = _context.Leads
                .Where(l => l.CompanyId == companyId)
                .Include(l => l.BusinessNature)
                .Include(l => l.Company)
                .AsQueryable();

            if (!string.IsNullOrEmpty(input.Filter))
            {
                query = query.Where(l => l.FullName.Contains(input.Filter) || l.Email.Contains(input.Filter));
            }

            var totalCount = await query.CountAsync();
            var leads = await query.Skip(input.SkipCount).Take(input.MaxResultCount).ToListAsync();

            var dtos = _mapper.Map<List<LeadDto>>(leads);

            return new PagedResultDto<LeadDto>(totalCount, dtos);
        }

        public async Task<List<LeadDto>> GetActiveLeadsAsync(int companyId)
        {
            var leads = await _context.Leads
                .Include(l => l.BusinessNature)
                .Include(l => l.Company)
                .Where(l => l.IsActive && l.CompanyId == companyId)
                .ToListAsync();
            return _mapper.Map<List<LeadDto>>(leads);
        }

        public async Task<LeadDto> GetAsync(int id, int companyId)
        {
            var lead = await _context.Leads
                .Include(l => l.BusinessNature)
                .Include(l => l.Company)
                .Where(l => l.Id == id && l.CompanyId == companyId)
                .FirstOrDefaultAsync();
            if (lead == null)
            {
                throw new ArgumentException("Lead not found");
            }
            return _mapper.Map<LeadDto>(lead);
        }

        public async Task<LeadDto> CreateAsync(CreateLeadDto input, int companyId)
        {
            var lead = _mapper.Map<Lead>(input);
            lead.CompanyId = companyId;
            lead.CreatedAt = DateTime.UtcNow;
            lead.UpdatedAt = DateTime.UtcNow;
            
            _context.Leads.Add(lead);
            await _context.SaveChangesAsync();
            
            // Reload with includes
            await _context.Entry(lead)
                .Reference(l => l.BusinessNature)
                .LoadAsync();
            await _context.Entry(lead)
                .Reference(l => l.Company)
                .LoadAsync();
                
            return _mapper.Map<LeadDto>(lead);
        }

        public async Task<LeadDto> UpdateAsync(UpdateLeadDto input, int companyId)
        {
            var lead = await _context.Leads
                .Include(l => l.BusinessNature)
                .Include(l => l.Company)
                .Where(l => l.Id == input.Id && l.CompanyId == companyId)
                .FirstOrDefaultAsync();
            if (lead == null)
            {
                throw new ArgumentException("Lead not found");
            }

            _mapper.Map(input, lead);
            lead.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return _mapper.Map<LeadDto>(lead);
        }

        public async Task DeleteAsync(int id, int companyId)
        {
            var lead = await _context.Leads
                .Where(l => l.Id == id && l.CompanyId == companyId)
                .FirstOrDefaultAsync();
            if (lead == null)
            {
                throw new ArgumentException("Lead not found");
            }

            _context.Leads.Remove(lead);
            await _context.SaveChangesAsync();
        }
    }
}