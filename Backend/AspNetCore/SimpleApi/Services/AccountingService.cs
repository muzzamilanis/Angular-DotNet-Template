using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SimpleApi.Data;
using SimpleApi.DTOs;
using SimpleApi.Models;

namespace SimpleApi.Services
{
    public interface IAccountingService
    {
        Task<VoucherDto> CreatePurchaseVoucherAsync(PropertyPurchase purchase, int userId);
        Task<VoucherDto> CreateSaleVoucherAsync(PropertySale sale, int userId);
        Task<Account> GetOrCreateAccountAsync(string accountCode, string accountName, string accountType, int companyId);
        Task PostVoucherAsync(int voucherId, int userId);
    }

    public class AccountingService : IAccountingService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public AccountingService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Create voucher for property purchase
        public async Task<VoucherDto> CreatePurchaseVoucherAsync(PropertyPurchase purchase, int userId)
        {
            var property = await _context.Properties.FindAsync(purchase.PropertyId);
            if (property == null) throw new ArgumentException("Property not found");

            // Get or create accounts
            var propertyAccount = await GetOrCreateAccountAsync("1500", "Property Assets", "Asset", purchase.CompanyId);
            var cashAccount = await GetOrCreateAccountAsync("1100", "Cash", "Asset", purchase.CompanyId);
            var bankAccount = await GetOrCreateAccountAsync("1200", "Bank", "Asset", purchase.CompanyId);
            var payableAccount = await GetOrCreateAccountAsync("2100", "Accounts Payable", "Liability", purchase.CompanyId);

            // Create voucher
            var voucher = new Voucher
            {
                VoucherNumber = $"PV-{DateTime.UtcNow:yyyyMMdd}-{purchase.Id}",
                VoucherType = purchase.PaymentMethod == "Cash" ? "Cash" : 
                              purchase.PaymentMethod == "Bank" ? "Bank" : "Journal",
                VoucherDate = purchase.PurchaseDate,
                ReferenceNumber = purchase.PurchaseNumber,
                Description = $"Property purchase from {purchase.SellerName} - {property.PropertyName}",
                TotalAmount = purchase.PurchasePrice,
                Status = "Posted",
                IsPosted = true,
                PostedDate = DateTime.UtcNow,
                PostedByUserId = userId,
                CompanyId = purchase.CompanyId
            };

            _context.Vouchers.Add(voucher);
            await _context.SaveChangesAsync();

            // Create journal entries
            var entries = new List<JournalEntry>();

            // Debit: Property Asset
            entries.Add(new JournalEntry
            {
                VoucherId = voucher.Id,
                AccountId = propertyAccount.Id,
                EntryType = "Debit",
                Amount = purchase.PurchasePrice,
                Description = $"Property acquisition - {property.PropertyName}",
                EntryDate = purchase.PurchaseDate,
                CompanyId = purchase.CompanyId
            });

            // Credit: Cash/Bank or Payable
            if (purchase.AdvancePayment > 0)
            {
                var paymentAccount = purchase.PaymentMethod == "Bank" ? bankAccount : cashAccount;
                entries.Add(new JournalEntry
                {
                    VoucherId = voucher.Id,
                    AccountId = paymentAccount.Id,
                    EntryType = "Credit",
                    Amount = purchase.AdvancePayment.Value,
                    Description = $"Advance payment for property - {property.PropertyName}",
                    EntryDate = purchase.PurchaseDate,
                    CompanyId = purchase.CompanyId
                });
            }

            if (purchase.RemainingPayment > 0)
            {
                entries.Add(new JournalEntry
                {
                    VoucherId = voucher.Id,
                    AccountId = payableAccount.Id,
                    EntryType = "Credit",
                    Amount = purchase.RemainingPayment.Value,
                    Description = $"Remaining payable for property - {property.PropertyName}",
                    EntryDate = purchase.PurchaseDate,
                    CompanyId = purchase.CompanyId
                });
            }

            _context.JournalEntries.AddRange(entries);
            await _context.SaveChangesAsync();

            // Update account balances
            await UpdateAccountBalancesAsync(entries);

            return _mapper.Map<VoucherDto>(voucher);
        }

        // Create voucher for property sale
        public async Task<VoucherDto> CreateSaleVoucherAsync(PropertySale sale, int userId)
        {
            var property = await _context.Properties.FindAsync(sale.PropertyId);
            if (property == null) throw new ArgumentException("Property not found");

            // Get or create accounts
            var propertyAccount = await GetOrCreateAccountAsync("1500", "Property Assets", "Asset", sale.CompanyId);
            var cashAccount = await GetOrCreateAccountAsync("1100", "Cash", "Asset", sale.CompanyId);
            var bankAccount = await GetOrCreateAccountAsync("1200", "Bank", "Asset", sale.CompanyId);
            var receivableAccount = await GetOrCreateAccountAsync("1300", "Accounts Receivable", "Asset", sale.CompanyId);
            var salesRevenueAccount = await GetOrCreateAccountAsync("4100", "Sales Revenue", "Revenue", sale.CompanyId);
            var profitLossAccount = sale.ProfitOrLoss >= 0 
                ? await GetOrCreateAccountAsync("4200", "Capital Gain", "Revenue", sale.CompanyId)
                : await GetOrCreateAccountAsync("5100", "Capital Loss", "Expense", sale.CompanyId);

            // Create voucher
            var voucher = new Voucher
            {
                VoucherNumber = $"SV-{DateTime.UtcNow:yyyyMMdd}-{sale.Id}",
                VoucherType = sale.PaymentMethod == "Cash" ? "Cash" : 
                              sale.PaymentMethod == "Bank" ? "Bank" : "Journal",
                VoucherDate = sale.SaleDate,
                ReferenceNumber = sale.SaleNumber,
                Description = $"Property sale to {sale.BuyerName} - {property.PropertyName}",
                TotalAmount = sale.SalePrice,
                Status = "Posted",
                IsPosted = true,
                PostedDate = DateTime.UtcNow,
                PostedByUserId = userId,
                CompanyId = sale.CompanyId
            };

            _context.Vouchers.Add(voucher);
            await _context.SaveChangesAsync();

            // Create journal entries
            var entries = new List<JournalEntry>();

            // Debit: Cash/Bank or Receivable
            if (sale.AdvanceReceived > 0)
            {
                var paymentAccount = sale.PaymentMethod == "Bank" ? bankAccount : cashAccount;
                entries.Add(new JournalEntry
                {
                    VoucherId = voucher.Id,
                    AccountId = paymentAccount.Id,
                    EntryType = "Debit",
                    Amount = sale.AdvanceReceived.Value,
                    Description = $"Advance received from property sale - {property.PropertyName}",
                    EntryDate = sale.SaleDate,
                    CompanyId = sale.CompanyId
                });
            }

            if (sale.RemainingReceivable > 0)
            {
                entries.Add(new JournalEntry
                {
                    VoucherId = voucher.Id,
                    AccountId = receivableAccount.Id,
                    EntryType = "Debit",
                    Amount = sale.RemainingReceivable.Value,
                    Description = $"Remaining receivable from property sale - {property.PropertyName}",
                    EntryDate = sale.SaleDate,
                    CompanyId = sale.CompanyId
                });
            }

            // Credit: Property Asset (at cost)
            entries.Add(new JournalEntry
            {
                VoucherId = voucher.Id,
                AccountId = propertyAccount.Id,
                EntryType = "Credit",
                Amount = sale.CostPrice,
                Description = $"Property disposal - {property.PropertyName}",
                EntryDate = sale.SaleDate,
                CompanyId = sale.CompanyId
            });

            // Profit or Loss
            if (sale.ProfitOrLoss != 0)
            {
                var entryType = sale.ProfitOrLoss > 0 ? "Credit" : "Debit";
                entries.Add(new JournalEntry
                {
                    VoucherId = voucher.Id,
                    AccountId = profitLossAccount.Id,
                    EntryType = entryType,
                    Amount = Math.Abs(sale.ProfitOrLoss),
                    Description = $"{(sale.ProfitOrLoss > 0 ? "Capital gain" : "Capital loss")} on property sale - {property.PropertyName}",
                    EntryDate = sale.SaleDate,
                    CompanyId = sale.CompanyId
                });
            }

            _context.JournalEntries.AddRange(entries);
            await _context.SaveChangesAsync();

            // Update account balances
            await UpdateAccountBalancesAsync(entries);

            return _mapper.Map<VoucherDto>(voucher);
        }

        // Get or create account
        public async Task<Account> GetOrCreateAccountAsync(string accountCode, string accountName, string accountType, int companyId)
        {
            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.AccountCode == accountCode && a.CompanyId == companyId);

            if (account == null)
            {
                account = new Account
                {
                    AccountCode = accountCode,
                    AccountName = accountName,
                    AccountType = accountType,
                    Level = 1,
                    OpeningBalance = 0,
                    CurrentBalance = 0,
                    IsActive = true,
                    CompanyId = companyId
                };

                _context.Accounts.Add(account);
                await _context.SaveChangesAsync();
            }

            return account;
        }

        // Post voucher
        public async Task PostVoucherAsync(int voucherId, int userId)
        {
            var voucher = await _context.Vouchers
                .Include(v => v.JournalEntries)
                .FirstOrDefaultAsync(v => v.Id == voucherId);

            if (voucher == null) throw new ArgumentException("Voucher not found");

            voucher.IsPosted = true;
            voucher.PostedDate = DateTime.UtcNow;
            voucher.PostedByUserId = userId;
            voucher.Status = "Posted";

            await _context.SaveChangesAsync();

            // Update account balances
            await UpdateAccountBalancesAsync(voucher.JournalEntries.ToList());
        }

        // Update account balances
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
    }
}
