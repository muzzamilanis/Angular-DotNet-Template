using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SimpleApi.Data;
using SimpleApi.DTOs;
using SimpleApi.Models;

namespace SimpleApi.Services
{
    public interface IPerformanceCriteriaService
    {
        Task<List<PerformanceCriteriaDto>> GetAllAsync(int companyId);
        Task<List<PerformanceCriteriaDto>> GetByUserRoleAsync(UserRole role, int companyId);
        Task<PerformanceCriteriaDto> GetByIdAsync(int id, int companyId);
        Task<PerformanceCriteriaDto> CreateAsync(CreatePerformanceCriteriaDto dto, int companyId);
        Task<PerformanceCriteriaDto> UpdateAsync(UpdatePerformanceCriteriaDto dto, int companyId);
        Task DeleteAsync(int id, int companyId);
    }

    public class PerformanceCriteriaService : IPerformanceCriteriaService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public PerformanceCriteriaService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<PerformanceCriteriaDto>> GetAllAsync(int companyId)
        {
            var criteria = await _context.PerformanceCriterias
                .Include(c => c.Company)
                .Where(c => c.CompanyId == companyId)
                .OrderBy(c => c.UserRole)
                .ThenBy(c => c.Name)
                .ToListAsync();

            return _mapper.Map<List<PerformanceCriteriaDto>>(criteria);
        }

        public async Task<List<PerformanceCriteriaDto>> GetByUserRoleAsync(UserRole role, int companyId)
        {
            var criteria = await _context.PerformanceCriterias
                .Include(c => c.Company)
                .Where(c => c.CompanyId == companyId && c.UserRole == role && c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();

            return _mapper.Map<List<PerformanceCriteriaDto>>(criteria);
        }

        public async Task<PerformanceCriteriaDto> GetByIdAsync(int id, int companyId)
        {
            var criteria = await _context.PerformanceCriterias
                .Include(c => c.Company)
                .FirstOrDefaultAsync(c => c.Id == id && c.CompanyId == companyId);

            if (criteria == null)
            {
                throw new ArgumentException("Performance criteria not found");
            }

            return _mapper.Map<PerformanceCriteriaDto>(criteria);
        }

        public async Task<PerformanceCriteriaDto> CreateAsync(CreatePerformanceCriteriaDto dto, int companyId)
        {
            var criteria = _mapper.Map<PerformanceCriteria>(dto);
            criteria.CompanyId = companyId;
            criteria.CreatedDate = DateTime.UtcNow;
            criteria.ModifiedDate = DateTime.UtcNow;

            _context.PerformanceCriterias.Add(criteria);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(criteria.Id, companyId);
        }

        public async Task<PerformanceCriteriaDto> UpdateAsync(UpdatePerformanceCriteriaDto dto, int companyId)
        {
            var criteria = await _context.PerformanceCriterias
                .FirstOrDefaultAsync(c => c.Id == dto.Id && c.CompanyId == companyId);

            if (criteria == null)
            {
                throw new ArgumentException("Performance criteria not found");
            }

            _mapper.Map(dto, criteria);
            criteria.ModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return await GetByIdAsync(criteria.Id, companyId);
        }

        public async Task DeleteAsync(int id, int companyId)
        {
            var criteria = await _context.PerformanceCriterias
                .FirstOrDefaultAsync(c => c.Id == id && c.CompanyId == companyId);

            if (criteria == null)
            {
                throw new ArgumentException("Performance criteria not found");
            }

            _context.PerformanceCriterias.Remove(criteria);
            await _context.SaveChangesAsync();
        }
    }
}
