using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SimpleApi.Data;
using SimpleApi.DTOs;
using SimpleApi.DTOs.CRM;
using SimpleApi.Models.CRM;

namespace SimpleApi.Services.CRM
{
    public interface ICrmFormSubmissionService
    {
        Task<PagedResultDto<CrmFormSubmissionDto>> GetAllAsync(PagedResultRequestDto input, int companyId);
        Task<List<CrmFormSubmissionDto>> GetSubmissionsByTemplateAsync(int templateId, int companyId);
        Task<List<CrmFormSubmissionDto>> GetSubmissionsByLeadAsync(int leadId, int companyId);
        Task<CrmFormSubmissionDto> GetAsync(int id, int companyId);
        Task<CrmFormSubmissionDto> CreateAsync(CreateCrmFormSubmissionDto input, int companyId);
        Task<CrmFormSubmissionDto> UpdateAsync(UpdateCrmFormSubmissionDto input, int companyId);
        Task DeleteAsync(int id, int companyId);
    }

    public class CrmFormSubmissionService : ICrmFormSubmissionService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CrmFormSubmissionService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedResultDto<CrmFormSubmissionDto>> GetAllAsync(PagedResultRequestDto input, int companyId)
        {
            var query = _context.CrmFormSubmissions
                .Where(s => s.CompanyId == companyId)
                .Include(s => s.FormTemplate)
                .Include(s => s.Lead)
                .Include(s => s.Contact)
                .Include(s => s.Company)
                .AsQueryable();

            var totalCount = await query.CountAsync();
            var submissions = await query.Skip(input.SkipCount).Take(input.MaxResultCount).ToListAsync();

            var dtos = _mapper.Map<List<CrmFormSubmissionDto>>(submissions);

            return new PagedResultDto<CrmFormSubmissionDto>(totalCount, dtos);
        }

        public async Task<List<CrmFormSubmissionDto>> GetSubmissionsByTemplateAsync(int templateId, int companyId)
        {
            var submissions = await _context.CrmFormSubmissions
                .Include(s => s.FormTemplate)
                .Include(s => s.Lead)
                .Include(s => s.Contact)
                .Include(s => s.Company)
                .Where(s => s.FormTemplateId == templateId && s.CompanyId == companyId)
                .ToListAsync();
            return _mapper.Map<List<CrmFormSubmissionDto>>(submissions);
        }

        public async Task<List<CrmFormSubmissionDto>> GetSubmissionsByLeadAsync(int leadId, int companyId)
        {
            var submissions = await _context.CrmFormSubmissions
                .Include(s => s.FormTemplate)
                .Include(s => s.Lead)
                .Include(s => s.Contact)
                .Include(s => s.Company)
                .Where(s => s.LeadId == leadId && s.CompanyId == companyId)
                .ToListAsync();
            return _mapper.Map<List<CrmFormSubmissionDto>>(submissions);
        }

        public async Task<CrmFormSubmissionDto> GetAsync(int id, int companyId)
        {
            var submission = await _context.CrmFormSubmissions
                .Include(s => s.FormTemplate)
                .Include(s => s.Lead)
                .Include(s => s.Contact)
                .Include(s => s.Company)
                .Where(s => s.Id == id && s.CompanyId == companyId)
                .FirstOrDefaultAsync();
            if (submission == null)
            {
                throw new ArgumentException("Form submission not found");
            }
            return _mapper.Map<CrmFormSubmissionDto>(submission);
        }

        public async Task<CrmFormSubmissionDto> CreateAsync(CreateCrmFormSubmissionDto input, int companyId)
        {
            var submission = _mapper.Map<CrmFormSubmission>(input);
            submission.CompanyId = companyId;
            submission.SubmittedAt = DateTime.UtcNow;
            
            _context.CrmFormSubmissions.Add(submission);
            await _context.SaveChangesAsync();
            
            // Reload with includes
            await _context.Entry(submission)
                .Reference(s => s.FormTemplate)
                .LoadAsync();
            await _context.Entry(submission)
                .Reference(s => s.Lead)
                .LoadAsync();
            await _context.Entry(submission)
                .Reference(s => s.Contact)
                .LoadAsync();
            await _context.Entry(submission)
                .Reference(s => s.Company)
                .LoadAsync();
                
            return _mapper.Map<CrmFormSubmissionDto>(submission);
        }

        public async Task<CrmFormSubmissionDto> UpdateAsync(UpdateCrmFormSubmissionDto input, int companyId)
        {
            var submission = await _context.CrmFormSubmissions
                .Include(s => s.FormTemplate)
                .Include(s => s.Lead)
                .Include(s => s.Contact)
                .Include(s => s.Company)
                .Where(s => s.Id == input.Id && s.CompanyId == companyId)
                .FirstOrDefaultAsync();
            if (submission == null)
            {
                throw new ArgumentException("Form submission not found");
            }

            _mapper.Map(input, submission);
            await _context.SaveChangesAsync();
            return _mapper.Map<CrmFormSubmissionDto>(submission);
        }

        public async Task DeleteAsync(int id, int companyId)
        {
            var submission = await _context.CrmFormSubmissions
                .Where(s => s.Id == id && s.CompanyId == companyId)
                .FirstOrDefaultAsync();
            if (submission == null)
            {
                throw new ArgumentException("Form submission not found");
            }

            _context.CrmFormSubmissions.Remove(submission);
            await _context.SaveChangesAsync();
        }
    }
}