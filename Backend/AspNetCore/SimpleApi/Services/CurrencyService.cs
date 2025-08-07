using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SimpleApi.Data;
using SimpleApi.DTOs;
using SimpleApi.Models;

namespace SimpleApi.Services
{
    public interface ICurrencyService
    {
        Task<List<CurrencyDto>> GetAllAsync(int companyId);
        Task<CurrencyDto> GetByIdAsync(int id, int companyId);
        Task<CurrencyDto> CreateAsync(CreateCurrencyDto input, int companyId);
        Task<CurrencyDto> UpdateAsync(UpdateCurrencyDto input, int companyId);
        Task DeleteAsync(int id, int companyId);
    }

    public class CurrencyService : ICurrencyService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CurrencyService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<CurrencyDto>> GetAllAsync(int companyId)
        {
            var currencies = await _context.Currencies
                .Where(c => c.CompanyId == companyId && c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();

            return _mapper.Map<List<CurrencyDto>>(currencies);
        }

        public async Task<CurrencyDto> GetByIdAsync(int id, int companyId)
        {
            var currency = await _context.Currencies
                .FirstOrDefaultAsync(c => c.Id == id && c.CompanyId == companyId);

            if (currency == null)
            {
                throw new ArgumentException("Currency not found");
            }

            return _mapper.Map<CurrencyDto>(currency);
        }

        public async Task<CurrencyDto> CreateAsync(CreateCurrencyDto input, int companyId)
        {
            var currency = _mapper.Map<Currency>(input);
            currency.CompanyId = companyId;
            currency.CreatedAt = DateTime.UtcNow;

            _context.Currencies.Add(currency);
            await _context.SaveChangesAsync();

            return _mapper.Map<CurrencyDto>(currency);
        }

        public async Task<CurrencyDto> UpdateAsync(UpdateCurrencyDto input, int companyId)
        {
            var currency = await _context.Currencies
                .FirstOrDefaultAsync(c => c.Id == input.Id && c.CompanyId == companyId);

            if (currency == null)
            {
                throw new ArgumentException("Currency not found");
            }

            _mapper.Map(input, currency);
            await _context.SaveChangesAsync();

            return _mapper.Map<CurrencyDto>(currency);
        }

        public async Task DeleteAsync(int id, int companyId)
        {
            var currency = await _context.Currencies
                .FirstOrDefaultAsync(c => c.Id == id && c.CompanyId == companyId);

            if (currency == null)
            {
                throw new ArgumentException("Currency not found");
            }

            _context.Currencies.Remove(currency);
            await _context.SaveChangesAsync();
        }
    }
}