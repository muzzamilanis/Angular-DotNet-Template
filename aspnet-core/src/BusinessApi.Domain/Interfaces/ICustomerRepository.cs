using BusinessApi.Domain.Entities;

namespace BusinessApi.Domain.Interfaces
{
    public interface ICustomerRepository
    {
        Task<List<Customer>> GetAllAsync(int skip = 0, int take = 10);
        Task<List<Customer>> GetActiveCustomersAsync();
        Task<Customer?> GetAsync(int id);
        Task<Customer> CreateAsync(Customer customer);
        Task<Customer> UpdateAsync(Customer customer);
        Task DeleteAsync(int id);
        Task<int> GetCountAsync();
    }
}