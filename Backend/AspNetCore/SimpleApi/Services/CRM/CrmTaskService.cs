using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SimpleApi.Data;
using SimpleApi.DTOs;
using SimpleApi.DTOs.CRM;
using SimpleApi.Models.CRM;

namespace SimpleApi.Services.CRM
{
    public interface ICrmTaskService
    {
        Task<PagedResultDto<CrmTaskDto>> GetAllAsync(PagedResultRequestDto input, int companyId);
        Task<List<CrmTaskDto>> GetActiveTasksAsync(int companyId);
        Task<CrmTaskDto> GetAsync(int id, int companyId);
        Task<CrmTaskDto> CreateAsync(CreateCrmTaskDto input, int companyId);
        Task<CrmTaskDto> UpdateAsync(UpdateCrmTaskDto input, int companyId);
        Task DeleteAsync(int id, int companyId);
    }

    public class CrmTaskService : ICrmTaskService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CrmTaskService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedResultDto<CrmTaskDto>> GetAllAsync(PagedResultRequestDto input, int companyId)
        {
            var query = _context.CrmTasks
                .Where(t => t.CompanyId == companyId)
                .Include(t => t.AssignedToUser)
                .Include(t => t.Lead)
                .Include(t => t.Contact)
                .Include(t => t.Deal)
                .Include(t => t.Company)
                .AsQueryable();

            if (!string.IsNullOrEmpty(input.Filter))
            {
                query = query.Where(t => t.Title.Contains(input.Filter) || 
                                   (t.Description != null && t.Description.Contains(input.Filter)));
            }

            var totalCount = await query.CountAsync();
            var tasks = await query.Skip(input.SkipCount).Take(input.MaxResultCount).ToListAsync();

            var dtos = _mapper.Map<List<CrmTaskDto>>(tasks);

            return new PagedResultDto<CrmTaskDto>(totalCount, dtos);
        }

        public async Task<List<CrmTaskDto>> GetActiveTasksAsync(int companyId)
        {
            var tasks = await _context.CrmTasks
                .Include(t => t.AssignedToUser)
                .Include(t => t.Lead)
                .Include(t => t.Contact)
                .Include(t => t.Deal)
                .Include(t => t.Company)
                .Where(t => t.IsActive && t.CompanyId == companyId)
                .ToListAsync();
            return _mapper.Map<List<CrmTaskDto>>(tasks);
        }

        public async Task<CrmTaskDto> GetAsync(int id, int companyId)
        {
            var task = await _context.CrmTasks
                .Include(t => t.AssignedToUser)
                .Include(t => t.Lead)
                .Include(t => t.Contact)
                .Include(t => t.Deal)
                .Include(t => t.Company)
                .Where(t => t.Id == id && t.CompanyId == companyId)
                .FirstOrDefaultAsync();
            if (task == null)
            {
                throw new ArgumentException("Task not found");
            }
            return _mapper.Map<CrmTaskDto>(task);
        }

        public async Task<CrmTaskDto> CreateAsync(CreateCrmTaskDto input, int companyId)
        {
            var task = _mapper.Map<CrmTask>(input);
            task.CompanyId = companyId;
            task.CreatedAt = DateTime.UtcNow;
            task.UpdatedAt = DateTime.UtcNow;
            
            _context.CrmTasks.Add(task);
            await _context.SaveChangesAsync();
            
            // Reload with includes
            await _context.Entry(task)
                .Reference(t => t.AssignedToUser)
                .LoadAsync();
            await _context.Entry(task)
                .Reference(t => t.Lead)
                .LoadAsync();
            await _context.Entry(task)
                .Reference(t => t.Contact)
                .LoadAsync();
            await _context.Entry(task)
                .Reference(t => t.Deal)
                .LoadAsync();
            await _context.Entry(task)
                .Reference(t => t.Company)
                .LoadAsync();
                
            return _mapper.Map<CrmTaskDto>(task);
        }

        public async Task<CrmTaskDto> UpdateAsync(UpdateCrmTaskDto input, int companyId)
        {
            var task = await _context.CrmTasks
                .Include(t => t.AssignedToUser)
                .Include(t => t.Lead)
                .Include(t => t.Contact)
                .Include(t => t.Deal)
                .Include(t => t.Company)
                .Where(t => t.Id == input.Id && t.CompanyId == companyId)
                .FirstOrDefaultAsync();
            if (task == null)
            {
                throw new ArgumentException("Task not found");
            }

            _mapper.Map(input, task);
            task.UpdatedAt = DateTime.UtcNow;
            
            // Update CompletedAt based on status
            if (input.Status == Models.CRM.Enums.CrmTaskStatus.Completed && task.CompletedAt == null)
            {
                task.CompletedAt = DateTime.UtcNow;
            }
            else if (input.Status != Models.CRM.Enums.CrmTaskStatus.Completed)
            {
                task.CompletedAt = null;
            }
            
            await _context.SaveChangesAsync();
            return _mapper.Map<CrmTaskDto>(task);
        }

        public async Task DeleteAsync(int id, int companyId)
        {
            var task = await _context.CrmTasks
                .Where(t => t.Id == id && t.CompanyId == companyId)
                .FirstOrDefaultAsync();
            if (task == null)
            {
                throw new ArgumentException("Task not found");
            }

            _context.CrmTasks.Remove(task);
            await _context.SaveChangesAsync();
        }
    }
}