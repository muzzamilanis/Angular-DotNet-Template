using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SimpleApi.Data;
using SimpleApi.DTOs;
using SimpleApi.DTOs.CRM;
using SimpleApi.Models.CRM;

namespace SimpleApi.Services.CRM
{
    public interface IDealService
    {
        Task<PagedResultDto<DealDto>> GetAllAsync(PagedResultRequestDto input, int companyId);
        Task<List<DealDto>> GetActiveDealsAsync(int companyId);
        Task<DealDto> GetAsync(int id, int companyId);
        Task<DealDto> CreateAsync(CreateDealDto input, int companyId);
        Task<DealDto> UpdateAsync(UpdateDealDto input, int companyId);
        Task DeleteAsync(int id, int companyId);
    }

    public class DealService : IDealService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public DealService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedResultDto<DealDto>> GetAllAsync(PagedResultRequestDto input, int companyId)
        {
            var query = _context.Deals
                .Where(d => d.CompanyId == companyId)
                .Include(d => d.Lead)
                .Include(d => d.Contact)
                .Include(d => d.Company)
                .AsQueryable();

            if (!string.IsNullOrEmpty(input.Filter))
            {
                query = query.Where(d => d.Title.Contains(input.Filter) || 
                                   (d.Description != null && d.Description.Contains(input.Filter)));
            }

            var totalCount = await query.CountAsync();
            var deals = await query.Skip(input.SkipCount).Take(input.MaxResultCount).ToListAsync();

            var dtos = _mapper.Map<List<DealDto>>(deals);

            return new PagedResultDto<DealDto>(totalCount, dtos);
        }

        public async Task<List<DealDto>> GetActiveDealsAsync(int companyId)
        {
            var deals = await _context.Deals
                .Include(d => d.Lead)
                .Include(d => d.Contact)
                .Include(d => d.Company)
                .Where(d => d.IsActive && d.CompanyId == companyId)
                .ToListAsync();
            return _mapper.Map<List<DealDto>>(deals);
        }

        public async Task<DealDto> GetAsync(int id, int companyId)
        {
            var deal = await _context.Deals
                .Include(d => d.Lead)
                .Include(d => d.Contact)
                .Include(d => d.Company)
                .Where(d => d.Id == id && d.CompanyId == companyId)
                .FirstOrDefaultAsync();
            if (deal == null)
            {
                throw new ArgumentException("Deal not found");
            }
            return _mapper.Map<DealDto>(deal);
        }

        public async Task<DealDto> CreateAsync(CreateDealDto input, int companyId)
        {
            var deal = _mapper.Map<Deal>(input);
            deal.CompanyId = companyId;
            deal.CreatedAt = DateTime.UtcNow;
            deal.UpdatedAt = DateTime.UtcNow;
            
            _context.Deals.Add(deal);
            await _context.SaveChangesAsync();
            
            // Reload with includes
            await _context.Entry(deal)
                .Reference(d => d.Lead)
                .LoadAsync();
            await _context.Entry(deal)
                .Reference(d => d.Contact)
                .LoadAsync();
            await _context.Entry(deal)
                .Reference(d => d.Company)
                .LoadAsync();
                
            return _mapper.Map<DealDto>(deal);
        }

        public async Task<DealDto> UpdateAsync(UpdateDealDto input, int companyId)
        {
            var deal = await _context.Deals
                .Include(d => d.Lead)
                .Include(d => d.Contact)
                .Include(d => d.Company)
                .Where(d => d.Id == input.Id && d.CompanyId == companyId)
                .FirstOrDefaultAsync();
            if (deal == null)
            {
                throw new ArgumentException("Deal not found");
            }

            _mapper.Map(input, deal);
            deal.UpdatedAt = DateTime.UtcNow;
            
            // Update ActualCloseDate based on status
            if (input.Status == Models.CRM.Enums.DealStatus.Won || input.Status == Models.CRM.Enums.DealStatus.Lost)
            {
                if (deal.ActualCloseDate == null)
                {
                    deal.ActualCloseDate = DateTime.UtcNow;
                }
            }
            
            await _context.SaveChangesAsync();
            return _mapper.Map<DealDto>(deal);
        }

        public async Task DeleteAsync(int id, int companyId)
        {
            var deal = await _context.Deals
                .Where(d => d.Id == id && d.CompanyId == companyId)
                .FirstOrDefaultAsync();
            if (deal == null)
            {
                throw new ArgumentException("Deal not found");
            }

            _context.Deals.Remove(deal);
            await _context.SaveChangesAsync();
        }
    }
}