using AutoMapper;
using BusinessApi.Application.DTOs;
using BusinessApi.Domain.Entities;
using BusinessApi.Domain.Interfaces;

namespace BusinessApi.Application.Services
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
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        public CustomerService(ICustomerRepository customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        public async Task<PagedResultDto<CustomerDto>> GetAllAsync(PagedResultRequestDto input)
        {
            var totalCount = await _customerRepository.GetCountAsync();
            var customers = await _customerRepository.GetAllAsync(input.SkipCount, input.MaxResultCount);
            
            var dtos = _mapper.Map<List<CustomerDto>>(customers);
            
            return new PagedResultDto<CustomerDto>(totalCount, dtos);
        }

        public async Task<List<CustomerDto>> GetActiveCustomersAsync()
        {
            var customers = await _customerRepository.GetActiveCustomersAsync();
            return _mapper.Map<List<CustomerDto>>(customers);
        }

        public async Task<CustomerDto> GetAsync(int id)
        {
            var customer = await _customerRepository.GetAsync(id);
            if (customer == null)
            {
                throw new InvalidOperationException("Customer not found");
            }
            return _mapper.Map<CustomerDto>(customer);
        }

        public async Task<CustomerDto> CreateAsync(CreateCustomerDto input)
        {
            var customer = _mapper.Map<Customer>(input);
            var createdCustomer = await _customerRepository.CreateAsync(customer);
            return _mapper.Map<CustomerDto>(createdCustomer);
        }

        public async Task<CustomerDto> UpdateAsync(UpdateCustomerDto input)
        {
            var customer = await _customerRepository.GetAsync(input.Id);
            if (customer == null)
            {
                throw new InvalidOperationException("Customer not found");
            }
            
            _mapper.Map(input, customer);
            var updatedCustomer = await _customerRepository.UpdateAsync(customer);
            return _mapper.Map<CustomerDto>(updatedCustomer);
        }

        public async Task DeleteAsync(int id)
        {
            await _customerRepository.DeleteAsync(id);
        }
    }
}