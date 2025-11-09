using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SimpleApi.Data;
using SimpleApi.DTOs;
using SimpleApi.Models;

namespace SimpleApi.Services
{
    public interface IVoucherService
    {
        Task<PagedResultDto<VoucherDto>> GetAllAsync(PagedResultRequestDto input, int companyId);
        Task<List<VoucherDto>> GetByTypeAsync(string voucherType, int companyId);
        Task<VoucherDto> GetAsync(int id, int companyId);
        Task<VoucherDto> CreateAsync(CreateVoucherDto input, int companyId, int userId);
        Task DeleteAsync(int id, int companyId);
    }

    public class VoucherService : IVoucherService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public VoucherService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedResultDto<VoucherDto>> GetAllAsync(PagedResultRequestDto input, int companyId)
        {
            var query = _context.Vouchers
                .Where(v => v.CompanyId == companyId)
                .Include(v => v.JournalEntries)
                    .ThenInclude(je => je.Account)
                .AsQueryable();
            
            var totalCount = await query.CountAsync();
            var vouchers = await query
                .OrderByDescending(v => v.VoucherDate)
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .ToListAsync();
            
            var dtos = _mapper.Map<List<VoucherDto>>(vouchers);
            
            return new PagedResultDto<VoucherDto>(totalCount, dtos);
        }

        public async Task<List<VoucherDto>> GetByTypeAsync(string voucherType, int companyId)
        {
            var vouchers = await _context.Vouchers
                .Where(v => v.CompanyId == companyId && v.VoucherType == voucherType)
                .Include(v => v.JournalEntries)
                    .ThenInclude(je => je.Account)
                .OrderByDescending(v => v.VoucherDate)
                .ToListAsync();
            
            return _mapper.Map<List<VoucherDto>>(vouchers);
        }

        public async Task<VoucherDto> GetAsync(int id, int companyId)
        {
            var voucher = await _context.Vouchers
                .Include(v => v.JournalEntries)
                    .ThenInclude(je => je.Account)
                .Where(v => v.Id == id && v.CompanyId == companyId)
                .FirstOrDefaultAsync();
            
            if (voucher == null)
            {
                throw new ArgumentException("Voucher not found");
            }
            
            return _mapper.Map<VoucherDto>(voucher);
        }

        public async Task<VoucherDto> CreateAsync(CreateVoucherDto input, int companyId, int userId)
        {
            // Validate that debits equal credits
            var totalDebits = input.JournalEntries.Where(e => e.EntryType == "Debit").Sum(e => e.Amount);
            var totalCredits = input.JournalEntries.Where(e => e.EntryType == "Credit").Sum(e => e.Amount);
            
            if (Math.Abs(totalDebits - totalCredits) > 0.01m)
            {
                throw new ArgumentException($"Debits ({totalDebits}) must equal Credits ({totalCredits})");
            }

            // Create voucher
            var voucher = new Voucher
            {
                VoucherNumber = input.VoucherNumber,
                VoucherType = input.VoucherType,
                VoucherDate = input.VoucherDate,
                ReferenceNumber = input.ReferenceNumber,
                Description = input.Description,
                TotalAmount = totalDebits, // or totalCredits, they're equal
                Status = "Posted",
                IsPosted = true,
                PostedDate = DateTime.UtcNow,
                PostedByUserId = userId,
                CompanyId = companyId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Vouchers.Add(voucher);
            await _context.SaveChangesAsync();

            // Create journal entries
            var journalEntries = new List<JournalEntry>();
            foreach (var entryDto in input.JournalEntries)
            {
                journalEntries.Add(new JournalEntry
                {
                    VoucherId = voucher.Id,
                    AccountId = entryDto.AccountId,
                    EntryType = entryDto.EntryType,
                    Amount = entryDto.Amount,
                    Description = entryDto.Description,
                    EntryDate = input.VoucherDate,
                    CompanyId = companyId,
                    CreatedAt = DateTime.UtcNow
                });
            }

            _context.JournalEntries.AddRange(journalEntries);
            await _context.SaveChangesAsync();

            // Update account balances
            await UpdateAccountBalancesAsync(journalEntries);

            // Reload voucher with entries
            var createdVoucher = await _context.Vouchers
                .Include(v => v.JournalEntries)
                    .ThenInclude(je => je.Account)
                .FirstOrDefaultAsync(v => v.Id == voucher.Id);

            return _mapper.Map<VoucherDto>(createdVoucher);
        }

        public async Task DeleteAsync(int id, int companyId)
        {
            var voucher = await _context.Vouchers
                .Include(v => v.JournalEntries)
                .Where(v => v.Id == id && v.CompanyId == companyId)
                .FirstOrDefaultAsync();
            
            if (voucher == null)
            {
                throw new ArgumentException("Voucher not found");
            }

            // Reverse account balances
            await ReverseAccountBalancesAsync(voucher.JournalEntries.ToList());

            // Remove journal entries and voucher
            _context.JournalEntries.RemoveRange(voucher.JournalEntries);
            _context.Vouchers.Remove(voucher);
            
            await _context.SaveChangesAsync();
        }

        private async Task UpdateAccountBalancesAsync(List<JournalEntry> entries)
        {
            foreach (var entry in entries)
            {
                var account = await _context.Accounts.FindAsync(entry.AccountId);
                if (account != null)
                {
                    if (entry.EntryType == "Debit")
                    {
                        account.CurrentBalance += entry.Amount;
                    }
                    else
                    {
                        account.CurrentBalance -= entry.Amount;
                    }
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task ReverseAccountBalancesAsync(List<JournalEntry> entries)
        {
            foreach (var entry in entries)
            {
                var account = await _context.Accounts.FindAsync(entry.AccountId);
                if (account != null)
                {
                    if (entry.EntryType == "Debit")
                    {
                        account.CurrentBalance -= entry.Amount;
                    }
                    else
                    {
                        account.CurrentBalance += entry.Amount;
                    }
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
