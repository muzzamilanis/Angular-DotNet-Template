using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SimpleApi.Data;
using SimpleApi.DTOs;
using SimpleApi.Models;

namespace SimpleApi.Services
{
    public interface ICustomerService
    {
        Task<PagedResultDto<CustomerDto>> GetAllAsync(PagedResultRequestDto input, int companyId);
        Task<List<CustomerDto>> GetActiveCustomersAsync(int companyId);
        Task<CustomerDto> GetAsync(int id, int companyId);
        Task<CustomerDto> CreateAsync(CreateCustomerDto input, int companyId);
        Task<CustomerDto> UpdateAsync(UpdateCustomerDto input, int companyId);
        Task DeleteAsync(int id, int companyId);
    }

    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CustomerService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedResultDto<CustomerDto>> GetAllAsync(PagedResultRequestDto input, int companyId)
        {
            var query = _context.Customers
                .Where(c => c.CompanyId == companyId)
                .Include(c => c.Company)
                .AsQueryable();
            
            var totalCount = await query.CountAsync();
            var customers = await query.Skip(input.SkipCount).Take(input.MaxResultCount).ToListAsync();
            
            var dtos = _mapper.Map<List<CustomerDto>>(customers);
            
            return new PagedResultDto<CustomerDto>(totalCount, dtos);
        }

        public async Task<List<CustomerDto>> GetActiveCustomersAsync(int companyId)
        {
            var customers = await _context.Customers
                .Include(c => c.Company)
                .Where(c => c.IsActive && c.CompanyId == companyId)
                .ToListAsync();
            return _mapper.Map<List<CustomerDto>>(customers);
        }

        public async Task<CustomerDto> GetAsync(int id, int companyId)
        {
            var customer = await _context.Customers
                .Where(c => c.Id == id && c.CompanyId == companyId)
                .FirstOrDefaultAsync();
            if (customer == null)
            {
                throw new ArgumentException("Customer not found");
            }
            return _mapper.Map<CustomerDto>(customer);
        }

        public async Task<CustomerDto> CreateAsync(CreateCustomerDto input, int companyId)
        {
            var customer = _mapper.Map<Customer>(input);
            customer.CompanyId = companyId;
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return _mapper.Map<CustomerDto>(customer);
        }

        public async Task<CustomerDto> UpdateAsync(UpdateCustomerDto input, int companyId)
        {
            var customer = await _context.Customers
                .Where(c => c.Id == input.Id && c.CompanyId == companyId)
                .FirstOrDefaultAsync();
            if (customer == null)
            {
                throw new ArgumentException("Customer not found");
            }
            
            _mapper.Map(input, customer);
            await _context.SaveChangesAsync();
            return _mapper.Map<CustomerDto>(customer);
        }

        public async Task DeleteAsync(int id, int companyId)
        {
            var customer = await _context.Customers
                .Where(c => c.Id == id && c.CompanyId == companyId)
                .FirstOrDefaultAsync();
            if (customer == null)
            {
                throw new ArgumentException("Customer not found");
            }
            
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
        }
    }
}