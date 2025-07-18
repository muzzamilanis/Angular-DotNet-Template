using BusinessApi.Domain.Entities;

namespace BusinessApi.Domain.Interfaces
{
    public interface ISaleOrderRepository
    {
        Task<List<SaleOrder>> GetAllAsync(int skip = 0, int take = 10);
        Task<SaleOrder?> GetAsync(int id);
        Task<SaleOrder> CreateAsync(SaleOrder saleOrder);
        Task<SaleOrder> UpdateAsync(SaleOrder saleOrder);
        Task DeleteAsync(int id);
        Task<int> GetCountAsync();
    }
}