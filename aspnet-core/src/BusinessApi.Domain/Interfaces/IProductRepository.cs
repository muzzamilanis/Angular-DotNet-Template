using BusinessApi.Domain.Entities;

namespace BusinessApi.Domain.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync(int skip = 0, int take = 10);
        Task<List<Product>> GetAllActiveProductsAsync();
        Task<Product?> GetAsync(int id);
        Task<Product> CreateAsync(Product product);
        Task<Product> UpdateAsync(Product product);
        Task DeleteAsync(int id);
        Task<int> GetCountAsync();
    }
}