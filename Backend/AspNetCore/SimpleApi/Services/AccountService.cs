using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SimpleApi.Data;
using SimpleApi.DTOs;
using SimpleApi.Models;

namespace SimpleApi.Services
{
    public interface IAccountService
    {
        Task<List<AccountDto>> GetAllAsync(int companyId);
        Task<List<AccountDto>> GetByTypeAsync(string accountType, int companyId);
        Task<AccountDto> GetAsync(int id, int companyId);
        Task<AccountDto> CreateAsync(CreateAccountDto input, int companyId);
        Task<AccountDto> UpdateAsync(UpdateAccountDto input, int companyId);
        Task DeleteAsync(int id, int companyId);
        Task InitializeDefaultAccountsAsync(int companyId);
    }

    public class AccountService : IAccountService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public AccountService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<AccountDto>> GetAllAsync(int companyId)
        {
            var accounts = await _context.Accounts
                .Where(a => a.CompanyId == companyId && a.IsActive)
                .OrderBy(a => a.AccountCode)
                .ToListAsync();
            return _mapper.Map<List<AccountDto>>(accounts);
        }

        public async Task<List<AccountDto>> GetByTypeAsync(string accountType, int companyId)
        {
            var accounts = await _context.Accounts
                .Where(a => a.CompanyId == companyId && a.AccountType == accountType && a.IsActive)
                .OrderBy(a => a.AccountCode)
                .ToListAsync();
            return _mapper.Map<List<AccountDto>>(accounts);
        }

        public async Task<AccountDto> GetAsync(int id, int companyId)
        {
            var account = await _context.Accounts
                .Where(a => a.Id == id && a.CompanyId == companyId)
                .FirstOrDefaultAsync();
            if (account == null)
            {
                throw new ArgumentException("Account not found");
            }
            return _mapper.Map<AccountDto>(account);
        }

        public async Task<AccountDto> CreateAsync(CreateAccountDto input, int companyId)
        {
            // Check if account code already exists
            var exists = await _context.Accounts
                .AnyAsync(a => a.AccountCode == input.AccountCode && a.CompanyId == companyId);
            if (exists)
            {
                throw new ArgumentException($"Account with code {input.AccountCode} already exists");
            }

            var account = _mapper.Map<Account>(input);
            account.CompanyId = companyId;
            account.CurrentBalance = input.OpeningBalance;
            account.CreatedAt = DateTime.UtcNow;
            
            // Calculate level based on parent
            if (input.ParentAccountId.HasValue)
            {
                var parent = await _context.Accounts.FindAsync(input.ParentAccountId.Value);
                if (parent != null)
                {
                    account.Level = parent.Level + 1;
                }
            }

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
            
            return _mapper.Map<AccountDto>(account);
        }

        public async Task<AccountDto> UpdateAsync(UpdateAccountDto input, int companyId)
        {
            var account = await _context.Accounts
                .Where(a => a.Id == input.Id && a.CompanyId == companyId)
                .FirstOrDefaultAsync();
            if (account == null)
            {
                throw new ArgumentException("Account not found");
            }
            
            // Check if new account code conflicts with another account
            var codeExists = await _context.Accounts
                .AnyAsync(a => a.AccountCode == input.AccountCode && a.CompanyId == companyId && a.Id != input.Id);
            if (codeExists)
            {
                throw new ArgumentException($"Account with code {input.AccountCode} already exists");
            }

            _mapper.Map(input, account);
            account.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return _mapper.Map<AccountDto>(account);
        }

        public async Task DeleteAsync(int id, int companyId)
        {
            var account = await _context.Accounts
                .Where(a => a.Id == id && a.CompanyId == companyId)
                .FirstOrDefaultAsync();
            if (account == null)
            {
                throw new ArgumentException("Account not found");
            }
            
            // Check if account has any journal entries
            var hasEntries = await _context.JournalEntries
                .AnyAsync(je => je.AccountId == id);
            if (hasEntries)
            {
                throw new ArgumentException("Cannot delete account with existing transactions");
            }

            account.IsActive = false;
            await _context.SaveChangesAsync();
        }

        public async Task InitializeDefaultAccountsAsync(int companyId)
        {
            // Check if accounts already exist
            var accountsExist = await _context.Accounts.AnyAsync(a => a.CompanyId == companyId);
            if (accountsExist) return;

            var defaultAccounts = new List<Account>
            {
                // Assets
                new Account { AccountCode = "1000", AccountName = "Assets", AccountType = "Asset", AccountCategory = "Main", Level = 1, OpeningBalance = 0, CurrentBalance = 0, CompanyId = companyId },
                new Account { AccountCode = "1100", AccountName = "Cash", AccountType = "Asset", AccountCategory = "Current Asset", Level = 2, OpeningBalance = 0, CurrentBalance = 0, CompanyId = companyId },
                new Account { AccountCode = "1200", AccountName = "Bank", AccountType = "Asset", AccountCategory = "Current Asset", Level = 2, OpeningBalance = 0, CurrentBalance = 0, CompanyId = companyId },
                new Account { AccountCode = "1300", AccountName = "Accounts Receivable", AccountType = "Asset", AccountCategory = "Current Asset", Level = 2, OpeningBalance = 0, CurrentBalance = 0, CompanyId = companyId },
                new Account { AccountCode = "1500", AccountName = "Property Assets", AccountType = "Asset", AccountCategory = "Fixed Asset", Level = 2, OpeningBalance = 0, CurrentBalance = 0, CompanyId = companyId },

                // Liabilities
                new Account { AccountCode = "2000", AccountName = "Liabilities", AccountType = "Liability", AccountCategory = "Main", Level = 1, OpeningBalance = 0, CurrentBalance = 0, CompanyId = companyId },
                new Account { AccountCode = "2100", AccountName = "Accounts Payable", AccountType = "Liability", AccountCategory = "Current Liability", Level = 2, OpeningBalance = 0, CurrentBalance = 0, CompanyId = companyId },

                // Equity
                new Account { AccountCode = "3000", AccountName = "Equity", AccountType = "Equity", AccountCategory = "Main", Level = 1, OpeningBalance = 0, CurrentBalance = 0, CompanyId = companyId },
                new Account { AccountCode = "3100", AccountName = "Capital", AccountType = "Equity", AccountCategory = "Owner's Equity", Level = 2, OpeningBalance = 0, CurrentBalance = 0, CompanyId = companyId },
                new Account { AccountCode = "3200", AccountName = "Retained Earnings", AccountType = "Equity", AccountCategory = "Owner's Equity", Level = 2, OpeningBalance = 0, CurrentBalance = 0, CompanyId = companyId },

                // Revenue
                new Account { AccountCode = "4000", AccountName = "Revenue", AccountType = "Revenue", AccountCategory = "Main", Level = 1, OpeningBalance = 0, CurrentBalance = 0, CompanyId = companyId },
                new Account { AccountCode = "4100", AccountName = "Sales Revenue", AccountType = "Revenue", AccountCategory = "Operating Revenue", Level = 2, OpeningBalance = 0, CurrentBalance = 0, CompanyId = companyId },
                new Account { AccountCode = "4200", AccountName = "Capital Gain", AccountType = "Revenue", AccountCategory = "Other Revenue", Level = 2, OpeningBalance = 0, CurrentBalance = 0, CompanyId = companyId },

                // Expenses
                new Account { AccountCode = "5000", AccountName = "Expenses", AccountType = "Expense", AccountCategory = "Main", Level = 1, OpeningBalance = 0, CurrentBalance = 0, CompanyId = companyId },
                new Account { AccountCode = "5100", AccountName = "Capital Loss", AccountType = "Expense", AccountCategory = "Other Expense", Level = 2, OpeningBalance = 0, CurrentBalance = 0, CompanyId = companyId }
            };

            _context.Accounts.AddRange(defaultAccounts);
            await _context.SaveChangesAsync();
        }
    }
}
