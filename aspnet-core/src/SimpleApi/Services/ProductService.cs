using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SimpleApi.Data;
using SimpleApi.DTOs;
using SimpleApi.Models;

namespace SimpleApi.Services
{
    public interface IProductService
    {
        Task<PagedResultDto<ProductDto>> GetAllAsync(PagedResultRequestDto input);
        Task<List<ProductDto>> GetAllActiveProductsAsync();
        Task<ProductDto> GetAsync(int id);
        Task<ProductDto> CreateAsync(CreateProductDto input);
        Task<ProductDto> UpdateAsync(UpdateProductDto input);
        Task DeleteAsync(int id);
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

        public async Task<PagedResultDto<ProductDto>> GetAllAsync(PagedResultRequestDto input)
        {
            var query = _context.Products.AsQueryable();
            
            var totalCount = await query.CountAsync();
            var products = await query.Skip(input.SkipCount).Take(input.MaxResultCount).ToListAsync();
            
            var dtos = _mapper.Map<List<ProductDto>>(products);
            
            return new PagedResultDto<ProductDto>(totalCount, dtos);
        }

        public async Task<List<ProductDto>> GetAllActiveProductsAsync()
        {
            var products = await _context.Products.Where(p => p.IsActive).ToListAsync();
            return _mapper.Map<List<ProductDto>>(products);
        }

        public async Task<ProductDto> GetAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                throw new InvalidOperationException("Product not found");
            }
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto input)
        {
            var product = _mapper.Map<Product>(input);
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> UpdateAsync(UpdateProductDto input)
        {
            var product = await _context.Products.FindAsync(input.Id);
            if (product == null)
            {
                throw new InvalidOperationException("Product not found");
            }
            
            _mapper.Map(input, product);
            await _context.SaveChangesAsync();
            return _mapper.Map<ProductDto>(product);
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                throw new InvalidOperationException("Product not found");
            }
            
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }
}