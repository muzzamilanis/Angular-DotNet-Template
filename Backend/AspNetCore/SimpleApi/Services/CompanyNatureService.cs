using SimpleApi.Data;
using Microsoft.EntityFrameworkCore;

namespace SimpleApi.Services
{
    public interface ICompanyNatureService
    {
        Task<bool> IsDigitalMarketingCompanyAsync(int companyId);
        Task<string> GetCompanyNatureAsync(int companyId);
        Task UpdateCompanyNatureAsync(int companyId, int businessNatureId);
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
            var company = await _context.Companies
                .Include(c => c.BusinessNature)
                .FirstOrDefaultAsync(c => c.Id == companyId);
            return company?.BusinessNature?.Name?.ToLower() == "digitalmarketingagency";
        }

        public async Task<string> GetCompanyNatureAsync(int companyId)
        {
            var company = await _context.Companies
                .Include(c => c.BusinessNature)
                .FirstOrDefaultAsync(c => c.Id == companyId);
            return company?.BusinessNature?.DisplayName ?? "General";
        }

        public async Task UpdateCompanyNatureAsync(int companyId, int businessNatureId)
        {
            var company = await _context.Companies.FindAsync(companyId);
            if (company != null)
            {
                company.BusinessNatureId = businessNatureId;
                await _context.SaveChangesAsync();
            }
        }
    }
}