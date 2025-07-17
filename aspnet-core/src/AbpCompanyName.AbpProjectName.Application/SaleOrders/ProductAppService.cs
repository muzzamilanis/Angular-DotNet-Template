using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using AbpCompanyName.AbpProjectName.SaleOrders;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AbpCompanyName.AbpProjectName.Application.SaleOrders
{
    public class ProductAppService : AsyncCrudAppService<Product, ProductDto, int, PagedResultRequestDto, CreateProductDto, ProductDto>
    {
        public ProductAppService(IRepository<Product, int> repository) : base(repository)
        {
        }

        public async Task<List<ProductDto>> GetAllActiveProductsAsync()
        {
            var products = await Repository.GetAllListAsync(p => p.IsActive);
            return ObjectMapper.Map<List<ProductDto>>(products);
        }

        protected override IQueryable<Product> CreateFilteredQuery(PagedResultRequestDto input)
        {
            return Repository.GetAllIncluding();
        }
    }
}