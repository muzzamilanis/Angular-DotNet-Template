using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SimpleApi.Data;
using SimpleApi.DTOs;
using SimpleApi.Models;

namespace SimpleApi.Services
{
    public interface IProductService
    {
        Task<PagedResultDto<ProductDto>> GetAllAsync(PagedResultRequestDto input, int companyId);
        Task<List<ProductDto>> GetAllActiveProductsAsync(int companyId);
        Task<ProductDto> GetAsync(int id, int companyId);
        Task<ProductDto> CreateAsync(CreateProductDto input, int companyId);
        Task<ProductDto> UpdateAsync(UpdateProductDto input, int companyId);
        Task DeleteAsync(int id, int companyId);
    }

    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ProductService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedResultDto<ProductDto>> GetAllAsync(PagedResultRequestDto input, int companyId)
        {
            var query = _context.Products
                .Where(p => p.CompanyId == companyId)
                .Include(p => p.Company)
                .AsQueryable();
            
            var totalCount = await query.CountAsync();
            var products = await query.Skip(input.SkipCount).Take(input.MaxResultCount).ToListAsync();
            
            var dtos = _mapper.Map<List<ProductDto>>(products);
            
            return new PagedResultDto<ProductDto>(totalCount, dtos);
        }

        public async Task<List<ProductDto>> GetAllActiveProductsAsync(int companyId)
        {
            var products = await _context.Products
                .Include(p => p.Company)
                .Where(p => p.IsActive && p.CompanyId == companyId)
                .ToListAsync();
            return _mapper.Map<List<ProductDto>>(products);
        }

        public async Task<ProductDto> GetAsync(int id, int companyId)
        {
            var product = await _context.Products
                .Where(p => p.Id == id && p.CompanyId == companyId)
                .FirstOrDefaultAsync();
            if (product == null)
            {
                throw new ArgumentException("Product not found");
            }
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto input, int companyId)
        {
            var product = _mapper.Map<Product>(input);
            product.CompanyId = companyId;
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> UpdateAsync(UpdateProductDto input, int companyId)
        {
            var product = await _context.Products
                .Where(p => p.Id == input.Id && p.CompanyId == companyId)
                .FirstOrDefaultAsync();
            if (product == null)
            {
                throw new ArgumentException("Product not found");
            }
            
            _mapper.Map(input, product);
            await _context.SaveChangesAsync();
            return _mapper.Map<ProductDto>(product);
        }

        public async Task DeleteAsync(int id, int companyId)
        {
            var product = await _context.Products
                .Where(p => p.Id == id && p.CompanyId == companyId)
                .FirstOrDefaultAsync();
            if (product == null)
            {
                throw new ArgumentException("Product not found");
            }
            
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }
}