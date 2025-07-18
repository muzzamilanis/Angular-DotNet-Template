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
    public class SaleOrderAppService : AsyncCrudAppService<SaleOrder, SaleOrderDto, int, PagedResultRequestDto, CreateSaleOrderDto, UpdateSaleOrderDto>
    {
        private readonly IRepository<SaleOrderItem, int> _saleOrderItemRepository;
        private readonly IRepository<Product, int> _productRepository;

        public SaleOrderAppService(
            IRepository<SaleOrder, int> repository,
            IRepository<SaleOrderItem, int> saleOrderItemRepository,
            IRepository<Product, int> productRepository) : base(repository)
        {
            _saleOrderItemRepository = saleOrderItemRepository;
            _productRepository = productRepository;
        }

        public async override Task<SaleOrderDto> GetAsync(EntityDto<int> input)
        {
            var saleOrder = await Repository.GetAllIncluding(so => so.Items)
                .Include(so => so.Items).ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(so => so.Id == input.Id);

            if (saleOrder == null)
            {
                throw new Abp.UI.UserFriendlyException("Sale order not found");
            }

            var dto = ObjectMapper.Map<SaleOrderDto>(saleOrder);
            
            // Map product names for items
            foreach (var item in dto.Items)
            {
                var product = await _productRepository.GetAsync(item.ProductId);
                item.ProductName = product.Name;
            }

            return dto;
        }

        public async override Task<PagedResultDto<SaleOrderDto>> GetAllAsync(PagedResultRequestDto input)
        {
            var query = Repository.GetAllIncluding(so => so.Items);
            
            var totalCount = await query.CountAsync();
            var items = await query.Skip(input.SkipCount).Take(input.MaxResultCount).ToListAsync();

            var dtos = ObjectMapper.Map<List<SaleOrderDto>>(items);
            
            return new PagedResultDto<SaleOrderDto>(totalCount, dtos);
        }

        public async override Task<SaleOrderDto> CreateAsync(CreateSaleOrderDto input)
        {
            var saleOrder = ObjectMapper.Map<SaleOrder>(input);
            
            // Calculate total amount
            decimal totalAmount = 0;
            foreach (var itemDto in input.Items)
            {
                totalAmount += itemDto.Quantity * itemDto.Price;
            }
            saleOrder.TotalAmount = totalAmount;

            await Repository.InsertAsync(saleOrder);
            await CurrentUnitOfWork.SaveChangesAsync();

            // Create items
            foreach (var itemDto in input.Items)
            {
                var item = ObjectMapper.Map<SaleOrderItem>(itemDto);
                item.SaleOrderId = saleOrder.Id;
                await _saleOrderItemRepository.InsertAsync(item);
            }

            await CurrentUnitOfWork.SaveChangesAsync();

            return await GetAsync(new EntityDto<int> { Id = saleOrder.Id });
        }

        public async override Task<SaleOrderDto> UpdateAsync(UpdateSaleOrderDto input)
        {
            var saleOrder = await Repository.GetAsync(input.Id);
            
            ObjectMapper.Map(input, saleOrder);

            // Delete existing items
            var existingItems = await _saleOrderItemRepository.GetAllListAsync(i => i.SaleOrderId == saleOrder.Id);
            foreach (var item in existingItems)
            {
                await _saleOrderItemRepository.DeleteAsync(item);
            }

            // Calculate total amount and create new items
            decimal totalAmount = 0;
            foreach (var itemDto in input.Items)
            {
                totalAmount += itemDto.Quantity * itemDto.Price;
                var item = ObjectMapper.Map<SaleOrderItem>(itemDto);
                item.SaleOrderId = saleOrder.Id;
                await _saleOrderItemRepository.InsertAsync(item);
            }
            saleOrder.TotalAmount = totalAmount;

            await Repository.UpdateAsync(saleOrder);
            await CurrentUnitOfWork.SaveChangesAsync();

            return await GetAsync(new EntityDto<int> { Id = saleOrder.Id });
        }

        protected override IQueryable<SaleOrder> CreateFilteredQuery(PagedResultRequestDto input)
        {
            return Repository.GetAllIncluding(so => so.Items);
        }
    }
}