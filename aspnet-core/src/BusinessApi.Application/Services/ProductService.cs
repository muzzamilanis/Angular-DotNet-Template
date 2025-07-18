using AutoMapper;
using BusinessApi.Application.DTOs;
using BusinessApi.Domain.Entities;
using BusinessApi.Domain.Interfaces;

namespace BusinessApi.Application.Services
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
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<PagedResultDto<ProductDto>> GetAllAsync(PagedResultRequestDto input)
        {
            var totalCount = await _productRepository.GetCountAsync();
            var products = await _productRepository.GetAllAsync(input.SkipCount, input.MaxResultCount);
            
            var dtos = _mapper.Map<List<ProductDto>>(products);
            
            return new PagedResultDto<ProductDto>(totalCount, dtos);
        }

        public async Task<List<ProductDto>> GetAllActiveProductsAsync()
        {
            var products = await _productRepository.GetAllActiveProductsAsync();
            return _mapper.Map<List<ProductDto>>(products);
        }

        public async Task<ProductDto> GetAsync(int id)
        {
            var product = await _productRepository.GetAsync(id);
            if (product == null)
            {
                throw new InvalidOperationException("Product not found");
            }
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto input)
        {
            var product = _mapper.Map<Product>(input);
            var createdProduct = await _productRepository.CreateAsync(product);
            return _mapper.Map<ProductDto>(createdProduct);
        }

        public async Task<ProductDto> UpdateAsync(UpdateProductDto input)
        {
            var product = await _productRepository.GetAsync(input.Id);
            if (product == null)
            {
                throw new InvalidOperationException("Product not found");
            }
            
            _mapper.Map(input, product);
            var updatedProduct = await _productRepository.UpdateAsync(product);
            return _mapper.Map<ProductDto>(updatedProduct);
        }

        public async Task DeleteAsync(int id)
        {
            await _productRepository.DeleteAsync(id);
        }
    }
}