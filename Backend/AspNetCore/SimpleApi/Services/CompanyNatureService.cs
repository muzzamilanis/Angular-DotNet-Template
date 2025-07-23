using SimpleApi.Data;
using Microsoft.EntityFrameworkCore;

namespace SimpleApi.Services
{
    public interface ICompanyNatureService
    {
        Task<bool> IsDigitalMarketingCompanyAsync(int companyId);
        Task<string> GetCompanyNatureAsync(int companyId);
        Task UpdateCompanyNatureAsync(int companyId, string companyNature);
    }

    public class CompanyNatureService : ICompanyNatureService
    {
        private readonly ApplicationDbContext _context;

        public CompanyNatureService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsDigitalMarketingCompanyAsync(int companyId)
        {
            var company = await _context.Companies.FindAsync(companyId);
            return company?.CompanyNature?.ToLower() == "digital marketing";
        }

        public async Task<string> GetCompanyNatureAsync(int companyId)
        {
            var company = await _context.Companies.FindAsync(companyId);
            return company?.CompanyNature ?? "General";
        }

        public async Task UpdateCompanyNatureAsync(int companyId, string companyNature)
        {
            var company = await _context.Companies.FindAsync(companyId);
            if (company != null)
            {
                company.CompanyNature = companyNature;
                await _context.SaveChangesAsync();
            }
        }
    }
}