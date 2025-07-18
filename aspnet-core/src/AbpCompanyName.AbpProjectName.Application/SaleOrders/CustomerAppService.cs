using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using AbpCompanyName.AbpProjectName.SaleOrders;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace AbpCompanyName.AbpProjectName.Application.SaleOrders
{
    public class CustomerAppService : AsyncCrudAppService<Customer, CustomerDto, int, PagedAndSortedResultRequestDto, CreateCustomerDto, UpdateCustomerDto>
    {
        public CustomerAppService(IRepository<Customer, int> repository) : base(repository)
        {
        }

        public override async Task<PagedResultDto<CustomerDto>> GetAllAsync(PagedAndSortedResultRequestDto input)
        {
            var query = Repository.GetAll();
            
            var totalCount = await query.CountAsync();
            var customers = await query.Skip(input.SkipCount).Take(input.MaxResultCount).ToListAsync();
            
            var dtos = ObjectMapper.Map<List<CustomerDto>>(customers);
            
            return new PagedResultDto<CustomerDto>(totalCount, dtos);
        }

        public async Task<ListResultDto<CustomerDto>> GetActiveCustomers()
        {
            var customers = await Repository.GetAllListAsync(c => c.IsActive);
            return new ListResultDto<CustomerDto>(
                ObjectMapper.Map<List<CustomerDto>>(customers)
            );
        }
    }
}