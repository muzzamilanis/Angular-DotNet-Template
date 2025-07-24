using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SimpleApi.Data;
using SimpleApi.DTOs;
using SimpleApi.Models;

namespace SimpleApi.Services
{
    public interface ICustomerService
    {
        Task<PagedResultDto<CustomerDto>> GetAllAsync(PagedResultRequestDto input);
        Task<List<CustomerDto>> GetActiveCustomersAsync();
        Task<CustomerDto> GetAsync(int id);
        Task<CustomerDto> CreateAsync(CreateCustomerDto input);
        Task<CustomerDto> UpdateAsync(UpdateCustomerDto input);
        Task DeleteAsync(int id);
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

        public async Task<PagedResultDto<CustomerDto>> GetAllAsync(PagedResultRequestDto input)
        {
            var query = _context.Customers.Include(c => c.Company).AsQueryable();
            
            var totalCount = await query.CountAsync();
            var customers = await query.Skip(input.SkipCount).Take(input.MaxResultCount).ToListAsync();
            
            var dtos = _mapper.Map<List<CustomerDto>>(customers);
            
            return new PagedResultDto<CustomerDto>(totalCount, dtos);
        }

        public async Task<List<CustomerDto>> GetActiveCustomersAsync()
        {
            var customers = await _context.Customers
                .Include(c => c.Company)
                .Where(c => c.IsActive)
                .ToListAsync();
            return _mapper.Map<List<CustomerDto>>(customers);
        }

        public async Task<CustomerDto> GetAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                throw new InvalidOperationException("Customer not found");
            }
            return _mapper.Map<CustomerDto>(customer);
        }

        public async Task<CustomerDto> CreateAsync(CreateCustomerDto input)
        {
            var customer = _mapper.Map<Customer>(input);
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return _mapper.Map<CustomerDto>(customer);
        }

        public async Task<CustomerDto> UpdateAsync(UpdateCustomerDto input)
        {
            var customer = await _context.Customers.FindAsync(input.Id);
            if (customer == null)
            {
                throw new InvalidOperationException("Customer not found");
            }
            
            _mapper.Map(input, customer);
            await _context.SaveChangesAsync();
            return _mapper.Map<CustomerDto>(customer);
        }

        public async Task DeleteAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                throw new InvalidOperationException("Customer not found");
            }
            
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
        }
    }
}