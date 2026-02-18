using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SimpleApi.Data;
using SimpleApi.DTOs;
using SimpleApi.Models;

namespace SimpleApi.Services
{
    public interface IUserPerformanceScoreService
    {
        Task<List<UserPerformanceScoreDto>> GetAllAsync(int companyId);
        Task<List<UserPerformanceScoreDto>> GetByUserIdAsync(int userId, int companyId);
        Task<UserPerformanceScoreDto> GetByIdAsync(int id, int companyId);
        Task<UserPerformanceScoreDto> CreateAsync(CreateUserPerformanceScoreDto dto, int companyId, int evaluatorUserId);
        Task<UserPerformanceScoreDto> UpdateAsync(UpdateUserPerformanceScoreDto dto, int companyId);
        Task DeleteAsync(int id, int companyId);
    }

    public class UserPerformanceScoreService : IUserPerformanceScoreService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UserPerformanceScoreService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<UserPerformanceScoreDto>> GetAllAsync(int companyId)
        {
            var scores = await _context.UserPerformanceScores
                .Include(s => s.User)
                .Include(s => s.PerformanceCriteria)
                .Include(s => s.Evaluator)
                .Where(s => s.User!.CompanyId == companyId)
                .OrderByDescending(s => s.EvaluationDate)
                .ToListAsync();

            return _mapper.Map<List<UserPerformanceScoreDto>>(scores);
        }

        public async Task<List<UserPerformanceScoreDto>> GetByUserIdAsync(int userId, int companyId)
        {
            // Validate user belongs to company
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId && u.CompanyId == companyId);
            if (user == null)
            {
                throw new ArgumentException("User not found or does not belong to your company");
            }

            var scores = await _context.UserPerformanceScores
                .Include(s => s.User)
                .Include(s => s.PerformanceCriteria)
                .Include(s => s.Evaluator)
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.EvaluationDate)
                .ToListAsync();

            return _mapper.Map<List<UserPerformanceScoreDto>>(scores);
        }

        public async Task<UserPerformanceScoreDto> GetByIdAsync(int id, int companyId)
        {
            var score = await _context.UserPerformanceScores
                .Include(s => s.User)
                .Include(s => s.PerformanceCriteria)
                .Include(s => s.Evaluator)
                .FirstOrDefaultAsync(s => s.Id == id && s.User!.CompanyId == companyId);

            if (score == null)
            {
                throw new ArgumentException("Performance score not found");
            }

            return _mapper.Map<UserPerformanceScoreDto>(score);
        }

        public async Task<UserPerformanceScoreDto> CreateAsync(CreateUserPerformanceScoreDto dto, int companyId, int evaluatorUserId)
        {
            // Validate user belongs to company
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == dto.UserId && u.CompanyId == companyId);
            if (user == null)
            {
                throw new ArgumentException("User not found or does not belong to your company");
            }

            // Validate criteria belongs to company
            var criteria = await _context.PerformanceCriterias
                .FirstOrDefaultAsync(c => c.Id == dto.PerformanceCriteriaId && c.CompanyId == companyId);
            if (criteria == null)
            {
                throw new ArgumentException("Performance criteria not found or does not belong to your company");
            }

            var score = _mapper.Map<UserPerformanceScore>(dto);
            score.EvaluatorUserId = evaluatorUserId;
            score.EvaluationDate = DateTime.UtcNow;
            score.CreatedDate = DateTime.UtcNow;
            score.ModifiedDate = DateTime.UtcNow;

            _context.UserPerformanceScores.Add(score);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(score.Id, companyId);
        }

        public async Task<UserPerformanceScoreDto> UpdateAsync(UpdateUserPerformanceScoreDto dto, int companyId)
        {
            var score = await _context.UserPerformanceScores
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == dto.Id && s.User!.CompanyId == companyId);

            if (score == null)
            {
                throw new ArgumentException("Performance score not found");
            }

            // Validate user belongs to company if being changed
            if (score.UserId != dto.UserId)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == dto.UserId && u.CompanyId == companyId);
                if (user == null)
                {
                    throw new ArgumentException("User not found or does not belong to your company");
                }
            }

            // Validate criteria belongs to company if being changed
            if (score.PerformanceCriteriaId != dto.PerformanceCriteriaId)
            {
                var criteria = await _context.PerformanceCriterias
                    .FirstOrDefaultAsync(c => c.Id == dto.PerformanceCriteriaId && c.CompanyId == companyId);
                if (criteria == null)
                {
                    throw new ArgumentException("Performance criteria not found or does not belong to your company");
                }
            }

            _mapper.Map(dto, score);
            score.ModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return await GetByIdAsync(score.Id, companyId);
        }

        public async Task DeleteAsync(int id, int companyId)
        {
            var score = await _context.UserPerformanceScores
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == id && s.User!.CompanyId == companyId);

            if (score == null)
            {
                throw new ArgumentException("Performance score not found");
            }

            _context.UserPerformanceScores.Remove(score);
            await _context.SaveChangesAsync();
        }
    }
}
