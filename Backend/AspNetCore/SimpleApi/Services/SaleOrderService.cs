using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SimpleApi.Data;
using SimpleApi.DTOs;
using SimpleApi.Models;

namespace SimpleApi.Services
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
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public SaleOrderService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedResultDto<SaleOrderDto>> GetAllAsync(PagedResultRequestDto input)
        {
            var query = _context.SaleOrders.Include(so => so.Items).ThenInclude(i => i.Product);
            
            var totalCount = await query.CountAsync();
            var saleOrders = await query.Skip(input.SkipCount).Take(input.MaxResultCount).ToListAsync();
            
            var dtos = _mapper.Map<List<SaleOrderDto>>(saleOrders);
            
            return new PagedResultDto<SaleOrderDto>(totalCount, dtos);
        }

        public async Task<SaleOrderDto> GetAsync(int id)
        {
            var saleOrder = await _context.SaleOrders
                .Include(so => so.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(so => so.Id == id);

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
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
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

                _context.SaleOrders.Add(saleOrder);
                await _context.SaveChangesAsync();

                // Create items after the sale order is saved (to get the ID)
                foreach (var itemDto in input.Items)
                {
                    var item = _mapper.Map<SaleOrderItem>(itemDto);
                    item.SaleOrderId = saleOrder.Id;
                    _context.SaleOrderItems.Add(item);
                }

                await _context.SaveChangesAsync();
                
                await transaction.CommitAsync();

                return await GetAsync(saleOrder.Id);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<SaleOrderDto> UpdateAsync(UpdateSaleOrderDto input)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                var saleOrder = await _context.SaleOrders
                    .Include(so => so.Items)
                    .FirstOrDefaultAsync(so => so.Id == input.Id);
                    
                if (saleOrder == null)
                {
                    throw new InvalidOperationException("Sale order not found");
                }
                
                // Update sale order properties
                _mapper.Map(input, saleOrder);

                // Delete existing items
                _context.SaleOrderItems.RemoveRange(saleOrder.Items);

                // Calculate total amount and create new items
                decimal totalAmount = 0;
                var newItems = new List<SaleOrderItem>();
                foreach (var itemDto in input.Items)
                {
                    totalAmount += itemDto.Quantity * itemDto.Price;
                    var item = _mapper.Map<SaleOrderItem>(itemDto);
                    item.SaleOrderId = saleOrder.Id;
                    newItems.Add(item);
                }
                saleOrder.TotalAmount = totalAmount;
                saleOrder.Items = newItems;

                await _context.SaveChangesAsync();
                
                await transaction.CommitAsync();

                return await GetAsync(saleOrder.Id);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                var saleOrder = await _context.SaleOrders
                    .Include(so => so.Items)
                    .FirstOrDefaultAsync(so => so.Id == id);
                    
                if (saleOrder == null)
                {
                    throw new InvalidOperationException("Sale order not found");
                }
                
                _context.SaleOrders.Remove(saleOrder);
                await _context.SaveChangesAsync();
                
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}