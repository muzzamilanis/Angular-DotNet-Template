using AutoMapper;
using BusinessApi.Application.DTOs;
using BusinessApi.Domain.Entities;
using BusinessApi.Domain.Interfaces;

namespace BusinessApi.Application.Services
{
    public interface ISaleOrderService
    {
        Task<PagedResultDto<SaleOrderDto>> GetAllAsync(PagedResultRequestDto input);
        Task<SaleOrderDto> GetAsync(int id);
        Task<SaleOrderDto> CreateAsync(CreateSaleOrderDto input);
        Task<SaleOrderDto> UpdateAsync(UpdateSaleOrderDto input);
        Task DeleteAsync(int id);
    }

    public class SaleOrderService : ISaleOrderService
    {
        private readonly ISaleOrderRepository _saleOrderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public SaleOrderService(ISaleOrderRepository saleOrderRepository, IProductRepository productRepository, IMapper mapper)
        {
            _saleOrderRepository = saleOrderRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<PagedResultDto<SaleOrderDto>> GetAllAsync(PagedResultRequestDto input)
        {
            var totalCount = await _saleOrderRepository.GetCountAsync();
            var saleOrders = await _saleOrderRepository.GetAllAsync(input.SkipCount, input.MaxResultCount);
            
            var dtos = _mapper.Map<List<SaleOrderDto>>(saleOrders);
            
            return new PagedResultDto<SaleOrderDto>(totalCount, dtos);
        }

        public async Task<SaleOrderDto> GetAsync(int id)
        {
            var saleOrder = await _saleOrderRepository.GetAsync(id);

            if (saleOrder == null)
            {
                throw new InvalidOperationException("Sale order not found");
            }

            var dto = _mapper.Map<SaleOrderDto>(saleOrder);
            
            // Map product names for items
            foreach (var item in dto.Items)
            {
                var saleOrderItem = saleOrder.Items.First(i => i.Id == item.Id);
                item.ProductName = saleOrderItem.Product?.Name;
            }

            return dto;
        }

        public async Task<SaleOrderDto> CreateAsync(CreateSaleOrderDto input)
        {
            var saleOrder = _mapper.Map<SaleOrder>(input);
            
            // Calculate total amount
            decimal totalAmount = 0;
            foreach (var itemDto in input.Items)
            {
                totalAmount += itemDto.Quantity * itemDto.Price;
            }
            saleOrder.TotalAmount = totalAmount;

            // Clear the items mapped by AutoMapper since we'll handle them manually
            saleOrder.Items.Clear();

            // Create items
            foreach (var itemDto in input.Items)
            {
                var item = _mapper.Map<SaleOrderItem>(itemDto);
                saleOrder.Items.Add(item);
            }

            var createdSaleOrder = await _saleOrderRepository.CreateAsync(saleOrder);
            return await GetAsync(createdSaleOrder.Id);
        }

        public async Task<SaleOrderDto> UpdateAsync(UpdateSaleOrderDto input)
        {
            var saleOrder = await _saleOrderRepository.GetAsync(input.Id);
                
            if (saleOrder == null)
            {
                throw new InvalidOperationException("Sale order not found");
            }
            
            // Update sale order properties
            _mapper.Map(input, saleOrder);

            // Clear existing items
            saleOrder.Items.Clear();

            // Calculate total amount and create new items
            decimal totalAmount = 0;
            foreach (var itemDto in input.Items)
            {
                totalAmount += itemDto.Quantity * itemDto.Price;
                var item = _mapper.Map<SaleOrderItem>(itemDto);
                saleOrder.Items.Add(item);
            }
            saleOrder.TotalAmount = totalAmount;

            var updatedSaleOrder = await _saleOrderRepository.UpdateAsync(saleOrder);
            return await GetAsync(updatedSaleOrder.Id);
        }

        public async Task DeleteAsync(int id)
        {
            await _saleOrderRepository.DeleteAsync(id);
        }
    }
}