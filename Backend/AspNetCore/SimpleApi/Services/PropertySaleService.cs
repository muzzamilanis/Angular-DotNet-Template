using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SimpleApi.Data;
using SimpleApi.DTOs;
using SimpleApi.Models;

namespace SimpleApi.Services
{
    public interface IPropertySaleService
    {
        Task<PagedResultDto<PropertySaleDto>> GetAllAsync(PagedResultRequestDto input, int companyId);
        Task<PropertySaleDto> GetAsync(int id, int companyId);
        Task<PropertySaleDto> CreateAsync(CreatePropertySaleDto input, int companyId, int userId);
        Task<PropertySaleDto> UpdateAsync(UpdatePropertySaleDto input, int companyId);
        Task DeleteAsync(int id, int companyId);
    }

    public class PropertySaleService : IPropertySaleService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAccountingService _accountingService;

        public PropertySaleService(ApplicationDbContext context, IMapper mapper, IAccountingService accountingService)
        {
            _context = context;
            _mapper = mapper;
            _accountingService = accountingService;
        }

        public async Task<PagedResultDto<PropertySaleDto>> GetAllAsync(PagedResultRequestDto input, int companyId)
        {
            var query = _context.PropertySales
                .Where(p => p.CompanyId == companyId)
                .Include(p => p.Property)
                .Include(p => p.Company)
                .AsQueryable();
            
            var totalCount = await query.CountAsync();
            var sales = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .ToListAsync();
            
            var dtos = _mapper.Map<List<PropertySaleDto>>(sales);
            
            return new PagedResultDto<PropertySaleDto>(totalCount, dtos);
        }

        public async Task<PropertySaleDto> GetAsync(int id, int companyId)
        {
            var sale = await _context.PropertySales
                .Include(p => p.Property)
                .Where(p => p.Id == id && p.CompanyId == companyId)
                .FirstOrDefaultAsync();
            if (sale == null)
            {
                throw new ArgumentException("Property sale not found");
            }
            return _mapper.Map<PropertySaleDto>(sale);
        }

        public async Task<PropertySaleDto> CreateAsync(CreatePropertySaleDto input, int companyId, int userId)
        {
            // Calculate profit/loss and remaining receivable
            var profitOrLoss = input.SalePrice - input.CostPrice;
            var remainingReceivable = input.SalePrice - (input.AdvanceReceived ?? 0);
            
            var sale = _mapper.Map<PropertySale>(input);
            sale.CompanyId = companyId;
            sale.ProfitOrLoss = profitOrLoss;
            sale.RemainingReceivable = remainingReceivable;
            sale.CreatedAt = DateTime.UtcNow;
            
            _context.PropertySales.Add(sale);
            await _context.SaveChangesAsync();

            // Create accounting voucher automatically
            try
            {
                var voucher = await _accountingService.CreateSaleVoucherAsync(sale, userId);
                sale.VoucherId = voucher.Id;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log error but don't fail the sale creation
                Console.WriteLine($"Error creating voucher for sale {sale.Id}: {ex.Message}");
            }
            
            // Update property status
            var property = await _context.Properties.FindAsync(sale.PropertyId);
            if (property != null)
            {
                property.PropertyStatus = "Sold";
                await _context.SaveChangesAsync();
            }
            
            return _mapper.Map<PropertySaleDto>(sale);
        }

        public async Task<PropertySaleDto> UpdateAsync(UpdatePropertySaleDto input, int companyId)
        {
            var sale = await _context.PropertySales
                .Where(p => p.Id == input.Id && p.CompanyId == companyId)
                .FirstOrDefaultAsync();
            if (sale == null)
            {
                throw new ArgumentException("Property sale not found");
            }
            
            // Calculate profit/loss and remaining receivable
            var profitOrLoss = input.SalePrice - input.CostPrice;
            var remainingReceivable = input.SalePrice - (input.AdvanceReceived ?? 0);
            
            _mapper.Map(input, sale);
            sale.ProfitOrLoss = profitOrLoss;
            sale.RemainingReceivable = remainingReceivable;
            sale.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return _mapper.Map<PropertySaleDto>(sale);
        }

        public async Task DeleteAsync(int id, int companyId)
        {
            var sale = await _context.PropertySales
                .Where(p => p.Id == id && p.CompanyId == companyId)
                .FirstOrDefaultAsync();
            if (sale == null)
            {
                throw new ArgumentException("Property sale not found");
            }
            
            sale.IsActive = false;
            await _context.SaveChangesAsync();
        }
    }
}
