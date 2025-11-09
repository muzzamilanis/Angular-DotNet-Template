using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SimpleApi.Data;
using SimpleApi.DTOs;
using SimpleApi.Models;

namespace SimpleApi.Services
{
    public interface IPropertyPurchaseService
    {
        Task<PagedResultDto<PropertyPurchaseDto>> GetAllAsync(PagedResultRequestDto input, int companyId);
        Task<PropertyPurchaseDto> GetAsync(int id, int companyId);
        Task<PropertyPurchaseDto> CreateAsync(CreatePropertyPurchaseDto input, int companyId, int userId);
        Task<PropertyPurchaseDto> UpdateAsync(UpdatePropertyPurchaseDto input, int companyId);
        Task DeleteAsync(int id, int companyId);
    }

    public class PropertyPurchaseService : IPropertyPurchaseService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAccountingService _accountingService;

        public PropertyPurchaseService(ApplicationDbContext context, IMapper mapper, IAccountingService accountingService)
        {
            _context = context;
            _mapper = mapper;
            _accountingService = accountingService;
        }

        public async Task<PagedResultDto<PropertyPurchaseDto>> GetAllAsync(PagedResultRequestDto input, int companyId)
        {
            var query = _context.PropertyPurchases
                .Where(p => p.CompanyId == companyId)
                .Include(p => p.Property)
                .Include(p => p.Company)
                .AsQueryable();
            
            var totalCount = await query.CountAsync();
            var purchases = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .ToListAsync();
            
            var dtos = _mapper.Map<List<PropertyPurchaseDto>>(purchases);
            
            return new PagedResultDto<PropertyPurchaseDto>(totalCount, dtos);
        }

        public async Task<PropertyPurchaseDto> GetAsync(int id, int companyId)
        {
            var purchase = await _context.PropertyPurchases
                .Include(p => p.Property)
                .Where(p => p.Id == id && p.CompanyId == companyId)
                .FirstOrDefaultAsync();
            if (purchase == null)
            {
                throw new ArgumentException("Property purchase not found");
            }
            return _mapper.Map<PropertyPurchaseDto>(purchase);
        }

        public async Task<PropertyPurchaseDto> CreateAsync(CreatePropertyPurchaseDto input, int companyId, int userId)
        {
            // Calculate remaining payment
            var remainingPayment = input.PurchasePrice - (input.AdvancePayment ?? 0);
            
            var purchase = _mapper.Map<PropertyPurchase>(input);
            purchase.CompanyId = companyId;
            purchase.RemainingPayment = remainingPayment;
            purchase.CreatedAt = DateTime.UtcNow;
            
            _context.PropertyPurchases.Add(purchase);
            await _context.SaveChangesAsync();

            // Create accounting voucher automatically
            try
            {
                var voucher = await _accountingService.CreatePurchaseVoucherAsync(purchase, userId);
                purchase.VoucherId = voucher.Id;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log error but don't fail the purchase creation
                Console.WriteLine($"Error creating voucher for purchase {purchase.Id}: {ex.Message}");
            }
            
            return _mapper.Map<PropertyPurchaseDto>(purchase);
        }

        public async Task<PropertyPurchaseDto> UpdateAsync(UpdatePropertyPurchaseDto input, int companyId)
        {
            var purchase = await _context.PropertyPurchases
                .Where(p => p.Id == input.Id && p.CompanyId == companyId)
                .FirstOrDefaultAsync();
            if (purchase == null)
            {
                throw new ArgumentException("Property purchase not found");
            }
            
            // Calculate remaining payment
            var remainingPayment = input.PurchasePrice - (input.AdvancePayment ?? 0);
            
            _mapper.Map(input, purchase);
            purchase.RemainingPayment = remainingPayment;
            purchase.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return _mapper.Map<PropertyPurchaseDto>(purchase);
        }

        public async Task DeleteAsync(int id, int companyId)
        {
            var purchase = await _context.PropertyPurchases
                .Where(p => p.Id == id && p.CompanyId == companyId)
                .FirstOrDefaultAsync();
            if (purchase == null)
            {
                throw new ArgumentException("Property purchase not found");
            }
            
            purchase.IsActive = false;
            await _context.SaveChangesAsync();
        }
    }
}
