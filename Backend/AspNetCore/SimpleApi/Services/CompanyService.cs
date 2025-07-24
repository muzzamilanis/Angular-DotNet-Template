using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SimpleApi.Data;
using SimpleApi.DTOs;
using SimpleApi.Models;

namespace SimpleApi.Services
{
    public interface ICompanyService
    {
        Task<CompanyDto> GetCurrentUserCompanyAsync(int companyId);
        Task<CompanyDto> GetByIdAsync(int id);
        Task<CompanyDto> CreateAsync(CreateCompanyDto input);
        Task<CompanyDto> UpdateAsync(UpdateCompanyDto input);
        Task DeleteAsync(int id);
    }

    public class CompanyService : ICompanyService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CompanyService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<CompanyDto> GetCurrentUserCompanyAsync(int companyId)
        {
            var company = await _context.Companies.FindAsync(companyId);
            if (company == null)
            {
                throw new ArgumentException("Company not found");
            }

            return _mapper.Map<CompanyDto>(company);
        }

        public async Task<CompanyDto> GetByIdAsync(int id)
        {
            var company = await _context.Companies.FindAsync(id);
            if (company == null)
            {
                throw new ArgumentException("Company not found");
            }

            return _mapper.Map<CompanyDto>(company);
        }

        public async Task<CompanyDto> CreateAsync(CreateCompanyDto input)
        {
            var company = _mapper.Map<Company>(input);
            company.CreatedAt = DateTime.UtcNow;

            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            return _mapper.Map<CompanyDto>(company);
        }

        public async Task<CompanyDto> UpdateAsync(UpdateCompanyDto input)
        {
            var company = await _context.Companies.FindAsync(input.Id);
            if (company == null)
            {
                throw new ArgumentException("Company not found");
            }

            _mapper.Map(input, company);
            await _context.SaveChangesAsync();

            return _mapper.Map<CompanyDto>(company);
        }

        public async Task DeleteAsync(int id)
        {
            var company = await _context.Companies.FindAsync(id);
            if (company == null)
            {
                throw new ArgumentException("Company not found");
            }

            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();
        }
    }
}