using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SimpleApi.Data;
using SimpleApi.DTOs;
using SimpleApi.Models;

namespace SimpleApi.Services
{
    public interface ISaleOrderService
    {
        Task<PagedResultDto<SaleOrderDto>> GetAllAsync(PagedResultRequestDto input, int companyId);
        Task<SaleOrderDto> GetAsync(int id, int companyId);
        Task<SaleOrderDto> CreateAsync(CreateSaleOrderDto input, int companyId);
        Task<SaleOrderDto> UpdateAsync(UpdateSaleOrderDto input, int companyId);
        Task DeleteAsync(int id, int companyId);
        Task<List<SaleOrder>> GetDeadlineApproachingOrdersAsync();
        bool IsDeadlineApproaching(SaleOrder order);
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

        public async Task<PagedResultDto<SaleOrderDto>> GetAllAsync(PagedResultRequestDto input, int companyId)
        {
            var query = _context.SaleOrders
                .Where(so => so.CompanyId == companyId)
                .Include(so => so.Items)
                .ThenInclude(i => i.Product)
                .Include(so => so.Project)
                .Include(so => so.ServiceType)
                .Include(so => so.Company)
                .AsQueryable();

            // Apply filtering
            if (!string.IsNullOrEmpty(input.Filter))
            {
                query = query.Where(so => so.OrderNumber.Contains(input.Filter) || 
                                         (so.CustomerName != null && so.CustomerName.Contains(input.Filter)) ||
                                         (so.ClientName != null && so.ClientName.Contains(input.Filter)));
            }

            // Apply sorting
            if (!string.IsNullOrEmpty(input.Sorting))
            {
                switch (input.Sorting.ToLower())
                {
                    case "ordernumber":
                        query = query.OrderBy(so => so.OrderNumber);
                        break;
                    case "ordernumber desc":
                        query = query.OrderByDescending(so => so.OrderNumber);
                        break;
                    case "orderdate":
                        query = query.OrderBy(so => so.OrderDate);
                        break;
                    case "orderdate desc":
                        query = query.OrderByDescending(so => so.OrderDate);
                        break;
                    case "customername":
                        query = query.OrderBy(so => so.CustomerName);
                        break;
                    case "customername desc":
                        query = query.OrderByDescending(so => so.CustomerName);
                        break;
                    case "totalamount":
                        query = query.OrderBy(so => so.TotalAmount);
                        break;
                    case "totalamount desc":
                        query = query.OrderByDescending(so => so.TotalAmount);
                        break;
                    default:
                        query = query.OrderByDescending(so => so.OrderDate);
                        break;
                }
            }
            else
            {
                query = query.OrderByDescending(so => so.OrderDate);
            }
            
            var totalCount = await query.CountAsync();
            var saleOrders = await query
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .ToListAsync();
            
            var dtos = _mapper.Map<List<SaleOrderDto>>(saleOrders);
            
            return new PagedResultDto<SaleOrderDto>(totalCount, dtos);
        }

        public async Task<SaleOrderDto> GetAsync(int id, int companyId)
        {
            var saleOrder = await _context.SaleOrders
                .Where(so => so.Id == id && so.CompanyId == companyId)
                .Include(so => so.Items)
                .ThenInclude(i => i.Product)
                .Include(so => so.Project)
                .Include(so => so.ServiceType)
                .FirstOrDefaultAsync();

            if (saleOrder == null)
            {
                throw new ArgumentException($"Sale order with ID {id} not found or access denied.");
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

        public async Task<SaleOrderDto> CreateAsync(CreateSaleOrderDto input, int companyId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                var saleOrder = _mapper.Map<SaleOrder>(input);
                saleOrder.CompanyId = companyId;

                // Validate Project belongs to the same company if specified
                if (input.ProjectId.HasValue)
                {
                    var project = await _context.Projects
                        .FirstOrDefaultAsync(p => p.Id == input.ProjectId.Value && p.CompanyId == companyId);
                    if (project == null)
                    {
                        throw new ArgumentException("Invalid project specified or access denied.");
                    }
                }
                
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
                    // Validate Product belongs to the same company
                    var product = await _context.Products
                        .FirstOrDefaultAsync(p => p.Id == itemDto.ProductId && p.CompanyId == companyId);
                    if (product == null)
                    {
                        throw new ArgumentException($"Invalid product specified (ID: {itemDto.ProductId}) or access denied.");
                    }

                    var item = _mapper.Map<SaleOrderItem>(itemDto);
                    item.SaleOrderId = saleOrder.Id;
                    _context.SaleOrderItems.Add(item);
                }

                await _context.SaveChangesAsync();
                
                await transaction.CommitAsync();

                return await GetAsync(saleOrder.Id, companyId);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<SaleOrderDto> UpdateAsync(UpdateSaleOrderDto input, int companyId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                var saleOrder = await _context.SaleOrders
                    .Where(so => so.Id == input.Id && so.CompanyId == companyId)
                    .Include(so => so.Items)
                    .FirstOrDefaultAsync();
                    
                if (saleOrder == null)
                {
                    throw new ArgumentException($"Sale order with ID {input.Id} not found or access denied.");
                }

                // Validate Project belongs to the same company if specified
                if (input.ProjectId.HasValue)
                {
                    var project = await _context.Projects
                        .FirstOrDefaultAsync(p => p.Id == input.ProjectId.Value && p.CompanyId == companyId);
                    if (project == null)
                    {
                        throw new ArgumentException("Invalid project specified or access denied.");
                    }
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
                    // Validate Product belongs to the same company
                    var product = await _context.Products
                        .FirstOrDefaultAsync(p => p.Id == itemDto.ProductId && p.CompanyId == companyId);
                    if (product == null)
                    {
                        throw new ArgumentException($"Invalid product specified (ID: {itemDto.ProductId}) or access denied.");
                    }

                    totalAmount += itemDto.Quantity * itemDto.Price;
                    var item = _mapper.Map<SaleOrderItem>(itemDto);
                    item.SaleOrderId = saleOrder.Id;
                    newItems.Add(item);
                }
                saleOrder.TotalAmount = totalAmount;
                saleOrder.Items = newItems;

                await _context.SaveChangesAsync();
                
                await transaction.CommitAsync();

                return await GetAsync(saleOrder.Id, companyId);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteAsync(int id, int companyId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                var saleOrder = await _context.SaleOrders
                    .Where(so => so.Id == id && so.CompanyId == companyId)
                    .Include(so => so.Items)
                    .FirstOrDefaultAsync();
                    
                if (saleOrder == null)
                {
                    throw new ArgumentException($"Sale order with ID {id} not found or access denied.");
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

        public async Task<List<SaleOrder>> GetDeadlineApproachingOrdersAsync()
        {
            var deadlineThreshold = DateTime.UtcNow.Date.AddDays(3);
            
            return await _context.SaleOrders
                .Where(so => so.Status == "In Progress" && 
                            so.Deadline.HasValue && 
                            so.Deadline.Value.ToUniversalTime() <= deadlineThreshold)
                .Include(so => so.Company)
                .ToListAsync();
        }

        public bool IsDeadlineApproaching(SaleOrder order)
        {
            if (!order.Deadline.HasValue || order.Status != "In Progress")
                return false;
                
            return order.Deadline.Value <= DateTime.Today.AddDays(3);
        }
    }
}