using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SimpleApi.Data;
using SimpleApi.DTOs;
using SimpleApi.Models;

namespace SimpleApi.Services
{
    public class ProjectClientService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ProjectClientService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<ProjectClientDto>> GetByProjectAsync(int projectId, int companyId)
        {
            var clients = await _context.ProjectClients
                .Where(c => c.ConstructionProjectId == projectId && c.CompanyId == companyId)
                .Include(c => c.ConstructionProject)
                .Include(c => c.LedgerEntries)
                .ToListAsync();

            return clients.Select(c =>
            {
                var dto = _mapper.Map<ProjectClientDto>(c);
                dto.ProjectName = c.ConstructionProject?.ProjectName;
                dto.LedgerBalance = c.LedgerEntries.LastOrDefault()?.Balance ?? 0;
                return dto;
            }).ToList();
        }

        public async Task<ProjectClientDto> GetAsync(int id, int companyId)
        {
            var client = await _context.ProjectClients
                .Include(c => c.ConstructionProject)
                .Include(c => c.LedgerEntries)
                .FirstOrDefaultAsync(c => c.Id == id && c.CompanyId == companyId);

            if (client == null)
                throw new ArgumentException($"Client with ID {id} not found or access denied.");

            var dto = _mapper.Map<ProjectClientDto>(client);
            dto.ProjectName = client.ConstructionProject?.ProjectName;
            dto.LedgerBalance = client.LedgerEntries.LastOrDefault()?.Balance ?? 0;
            return dto;
        }

        public async Task<ProjectClientDto> CreateAsync(CreateProjectClientDto input, int companyId)
        {
            var client = _mapper.Map<ProjectClient>(input);
            client.CompanyId = companyId;
            client.CreatedAt = DateTime.UtcNow;

            _context.ProjectClients.Add(client);
            await _context.SaveChangesAsync();

            return await GetAsync(client.Id, companyId);
        }

        public async Task<ProjectClientDto> UpdateAsync(UpdateProjectClientDto input, int companyId)
        {
            var client = await _context.ProjectClients
                .FirstOrDefaultAsync(c => c.Id == input.Id && c.CompanyId == companyId);

            if (client == null)
                throw new ArgumentException($"Client with ID {input.Id} not found or access denied.");

            _mapper.Map(input, client);
            client.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return await GetAsync(client.Id, companyId);
        }

        public async Task DeleteAsync(int id, int companyId)
        {
            var client = await _context.ProjectClients
                .Include(c => c.LedgerEntries)
                .FirstOrDefaultAsync(c => c.Id == id && c.CompanyId == companyId);

            if (client == null)
                throw new ArgumentException($"Client with ID {id} not found or access denied.");

            if (client.LedgerEntries.Any())
            {
                client.IsActive = false;
                client.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                _context.ProjectClients.Remove(client);
            }

            await _context.SaveChangesAsync();
        }
    }

    public class ProjectInvestorService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ProjectInvestorService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<ProjectInvestorDto>> GetByProjectAsync(int projectId, int companyId)
        {
            var investors = await _context.ProjectInvestors
                .Where(i => i.ConstructionProjectId == projectId && i.CompanyId == companyId)
                .Include(i => i.ConstructionProject)
                .Include(i => i.LedgerEntries)
                .ToListAsync();

            return investors.Select(i =>
            {
                var dto = _mapper.Map<ProjectInvestorDto>(i);
                dto.ProjectName = i.ConstructionProject?.ProjectName;
                dto.LedgerBalance = i.LedgerEntries.LastOrDefault()?.Balance ?? 0;
                return dto;
            }).ToList();
        }

        public async Task<ProjectInvestorDto> GetAsync(int id, int companyId)
        {
            var investor = await _context.ProjectInvestors
                .Include(i => i.ConstructionProject)
                .Include(i => i.LedgerEntries)
                .FirstOrDefaultAsync(i => i.Id == id && i.CompanyId == companyId);

            if (investor == null)
                throw new ArgumentException($"Investor with ID {id} not found or access denied.");

            var dto = _mapper.Map<ProjectInvestorDto>(investor);
            dto.ProjectName = investor.ConstructionProject?.ProjectName;
            dto.LedgerBalance = investor.LedgerEntries.LastOrDefault()?.Balance ?? 0;
            return dto;
        }

        public async Task<ProjectInvestorDto> CreateAsync(CreateProjectInvestorDto input, int companyId)
        {
            var investor = _mapper.Map<ProjectInvestor>(input);
            investor.CompanyId = companyId;
            investor.CreatedAt = DateTime.UtcNow;

            _context.ProjectInvestors.Add(investor);
            await _context.SaveChangesAsync();

            return await GetAsync(investor.Id, companyId);
        }

        public async Task<ProjectInvestorDto> UpdateAsync(UpdateProjectInvestorDto input, int companyId)
        {
            var investor = await _context.ProjectInvestors
                .FirstOrDefaultAsync(i => i.Id == input.Id && i.CompanyId == companyId);

            if (investor == null)
                throw new ArgumentException($"Investor with ID {input.Id} not found or access denied.");

            _mapper.Map(input, investor);
            investor.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return await GetAsync(investor.Id, companyId);
        }

        public async Task DeleteAsync(int id, int companyId)
        {
            var investor = await _context.ProjectInvestors
                .Include(i => i.LedgerEntries)
                .FirstOrDefaultAsync(i => i.Id == id && i.CompanyId == companyId);

            if (investor == null)
                throw new ArgumentException($"Investor with ID {id} not found or access denied.");

            if (investor.LedgerEntries.Any())
            {
                investor.IsActive = false;
                investor.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                _context.ProjectInvestors.Remove(investor);
            }

            await _context.SaveChangesAsync();
        }
    }

    public class ProjectPaymentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ProjectPaymentService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<ProjectPaymentDto>> GetByProjectAsync(int projectId, int companyId)
        {
            var payments = await _context.ProjectPayments
                .Where(p => p.ConstructionProjectId == projectId && p.CompanyId == companyId)
                .Include(p => p.ConstructionProject)
                .Include(p => p.ProjectClient)
                .Include(p => p.ProjectInvestor)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();

            return payments.Select(p =>
            {
                var dto = _mapper.Map<ProjectPaymentDto>(p);
                dto.ProjectName = p.ConstructionProject?.ProjectName;
                dto.ClientName = p.ProjectClient?.ClientName;
                dto.InvestorName = p.ProjectInvestor?.InvestorName;
                return dto;
            }).ToList();
        }

        public async Task<ProjectPaymentDto> CreateAsync(CreateProjectPaymentDto input, int companyId, int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Generate payment number
                var lastPayment = await _context.ProjectPayments
                    .Where(p => p.CompanyId == companyId)
                    .OrderByDescending(p => p.Id)
                    .FirstOrDefaultAsync();

                var paymentNumber = $"PAY-{DateTime.UtcNow:yyyyMMdd}-{(lastPayment?.Id ?? 0) + 1:D4}";

                var payment = _mapper.Map<ProjectPayment>(input);
                payment.PaymentNumber = paymentNumber;
                payment.CompanyId = companyId;
                payment.CreatedAt = DateTime.UtcNow;

                _context.ProjectPayments.Add(payment);
                await _context.SaveChangesAsync();

                // Create ledger entries
                await CreateLedgerEntriesForPaymentAsync(payment, companyId);

                // Update project actual cost
                await UpdateProjectFinancialsAsync(input.ConstructionProjectId, companyId);

                await transaction.CommitAsync();

                return await GetAsync(payment.Id, companyId);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task CreateLedgerEntriesForPaymentAsync(ProjectPayment payment, int companyId)
        {
            // Client ledger entry
            if (payment.PaymentSource == PaymentSource.Client && payment.ProjectClientId.HasValue)
            {
                var lastClientEntry = await _context.ClientLedgerEntries
                    .Where(e => e.ProjectClientId == payment.ProjectClientId.Value)
                    .OrderByDescending(e => e.EntryDate)
                    .FirstOrDefaultAsync();

                var clientEntry = new ClientLedgerEntry
                {
                    EntryNumber = $"CL-{DateTime.UtcNow:yyyyMMddHHmmss}",
                    EntryDate = payment.PaymentDate,
                    EntryType = "Credit",
                    Amount = payment.Amount,
                    Balance = (lastClientEntry?.Balance ?? 0) + payment.Amount,
                    Description = payment.Description ?? $"Payment received: {payment.PaymentNumber}",
                    ReferenceNumber = payment.PaymentNumber,
                    ProjectClientId = payment.ProjectClientId.Value,
                    ConstructionProjectId = payment.ConstructionProjectId,
                    ProjectPaymentId = payment.Id,
                    CompanyId = companyId,
                    CreatedAt = DateTime.UtcNow
                };

                _context.ClientLedgerEntries.Add(clientEntry);
            }

            // Investor ledger entry
            if (payment.PaymentSource == PaymentSource.Investor && payment.ProjectInvestorId.HasValue)
            {
                var lastInvestorEntry = await _context.InvestorLedgerEntries
                    .Where(e => e.ProjectInvestorId == payment.ProjectInvestorId.Value)
                    .OrderByDescending(e => e.EntryDate)
                    .FirstOrDefaultAsync();

                var investorEntry = new InvestorLedgerEntry
                {
                    EntryNumber = $"IL-{DateTime.UtcNow:yyyyMMddHHmmss}",
                    EntryDate = payment.PaymentDate,
                    EntryType = "Credit",
                    Amount = payment.Amount,
                    Balance = (lastInvestorEntry?.Balance ?? 0) + payment.Amount,
                    Description = payment.Description ?? $"Investment received: {payment.PaymentNumber}",
                    ReferenceNumber = payment.PaymentNumber,
                    ProjectInvestorId = payment.ProjectInvestorId.Value,
                    ConstructionProjectId = payment.ConstructionProjectId,
                    ProjectPaymentId = payment.Id,
                    CompanyId = companyId,
                    CreatedAt = DateTime.UtcNow
                };

                _context.InvestorLedgerEntries.Add(investorEntry);
            }

            // Project ledger entry
            var lastProjectEntry = await _context.ProjectLedgerEntries
                .Where(e => e.ConstructionProjectId == payment.ConstructionProjectId)
                .OrderByDescending(e => e.EntryDate)
                .FirstOrDefaultAsync();

            var projectEntry = new ProjectLedgerEntry
            {
                EntryNumber = $"PL-{DateTime.UtcNow:yyyyMMddHHmmss}",
                EntryDate = payment.PaymentDate,
                TransactionType = "Payment",
                EntryType = "Credit",
                Amount = payment.Amount,
                Balance = (lastProjectEntry?.Balance ?? 0) + payment.Amount,
                Description = payment.Description ?? $"Payment received: {payment.PaymentNumber}",
                ReferenceNumber = payment.PaymentNumber,
                ConstructionProjectId = payment.ConstructionProjectId,
                ProjectPaymentId = payment.Id,
                CompanyId = companyId,
                CreatedAt = DateTime.UtcNow
            };

            _context.ProjectLedgerEntries.Add(projectEntry);
            await _context.SaveChangesAsync();
        }

        private async Task UpdateProjectFinancialsAsync(int projectId, int companyId)
        {
            var project = await _context.ConstructionProjects
                .Include(p => p.ProjectPayments)
                .Include(p => p.ProjectExpenses)
                .FirstOrDefaultAsync(p => p.Id == projectId && p.CompanyId == companyId);

            if (project != null)
            {
                project.ActualCost = project.ProjectExpenses.Where(e => e.IsActive).Sum(e => e.Amount);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ProjectPaymentDto> GetAsync(int id, int companyId)
        {
            var payment = await _context.ProjectPayments
                .Include(p => p.ConstructionProject)
                .Include(p => p.ProjectClient)
                .Include(p => p.ProjectInvestor)
                .FirstOrDefaultAsync(p => p.Id == id && p.CompanyId == companyId);

            if (payment == null)
                throw new ArgumentException($"Payment with ID {id} not found or access denied.");

            var dto = _mapper.Map<ProjectPaymentDto>(payment);
            dto.ProjectName = payment.ConstructionProject?.ProjectName;
            dto.ClientName = payment.ProjectClient?.ClientName;
            dto.InvestorName = payment.ProjectInvestor?.InvestorName;
            return dto;
        }
    }

    public class ProjectExpenseService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ProjectExpenseService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<ProjectExpenseDto>> GetByProjectAsync(int projectId, int companyId)
        {
            var expenses = await _context.ProjectExpenses
                .Where(e => e.ConstructionProjectId == projectId && e.CompanyId == companyId)
                .Include(e => e.ConstructionProject)
                .OrderByDescending(e => e.ExpenseDate)
                .ToListAsync();

            return expenses.Select(e =>
            {
                var dto = _mapper.Map<ProjectExpenseDto>(e);
                dto.ProjectName = e.ConstructionProject?.ProjectName;
                return dto;
            }).ToList();
        }

        public async Task<ProjectExpenseDto> CreateAsync(CreateProjectExpenseDto input, int companyId, int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Generate expense number
                var lastExpense = await _context.ProjectExpenses
                    .Where(e => e.CompanyId == companyId)
                    .OrderByDescending(e => e.Id)
                    .FirstOrDefaultAsync();

                var expenseNumber = $"EXP-{DateTime.UtcNow:yyyyMMdd}-{(lastExpense?.Id ?? 0) + 1:D4}";

                var expense = _mapper.Map<ProjectExpense>(input);
                expense.ExpenseNumber = expenseNumber;
                expense.CompanyId = companyId;
                expense.CreatedAt = DateTime.UtcNow;

                _context.ProjectExpenses.Add(expense);
                await _context.SaveChangesAsync();

                // Create ledger entry
                await CreateLedgerEntryForExpenseAsync(expense, companyId);

                // Update project actual cost
                await UpdateProjectFinancialsAsync(input.ConstructionProjectId, companyId);

                await transaction.CommitAsync();

                return await GetAsync(expense.Id, companyId);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task CreateLedgerEntryForExpenseAsync(ProjectExpense expense, int companyId)
        {
            var lastProjectEntry = await _context.ProjectLedgerEntries
                .Where(e => e.ConstructionProjectId == expense.ConstructionProjectId)
                .OrderByDescending(e => e.EntryDate)
                .FirstOrDefaultAsync();

            var projectEntry = new ProjectLedgerEntry
            {
                EntryNumber = $"PL-{DateTime.UtcNow:yyyyMMddHHmmss}",
                EntryDate = expense.ExpenseDate,
                TransactionType = "Expense",
                EntryType = "Debit",
                Amount = expense.Amount,
                Balance = (lastProjectEntry?.Balance ?? 0) - expense.Amount,
                Description = expense.Description ?? $"Expense: {expense.ExpenseCategory}",
                ReferenceNumber = expense.ExpenseNumber,
                ConstructionProjectId = expense.ConstructionProjectId,
                ProjectExpenseId = expense.Id,
                CompanyId = companyId,
                CreatedAt = DateTime.UtcNow
            };

            _context.ProjectLedgerEntries.Add(projectEntry);
            await _context.SaveChangesAsync();
        }

        private async Task UpdateProjectFinancialsAsync(int projectId, int companyId)
        {
            var project = await _context.ConstructionProjects
                .Include(p => p.ProjectExpenses)
                .FirstOrDefaultAsync(p => p.Id == projectId && p.CompanyId == companyId);

            if (project != null)
            {
                project.ActualCost = project.ProjectExpenses.Where(e => e.IsActive).Sum(e => e.Amount);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ProjectExpenseDto> GetAsync(int id, int companyId)
        {
            var expense = await _context.ProjectExpenses
                .Include(e => e.ConstructionProject)
                .FirstOrDefaultAsync(e => e.Id == id && e.CompanyId == companyId);

            if (expense == null)
                throw new ArgumentException($"Expense with ID {id} not found or access denied.");

            var dto = _mapper.Map<ProjectExpenseDto>(expense);
            dto.ProjectName = expense.ConstructionProject?.ProjectName;
            return dto;
        }
    }

    public class ConstructionLedgerService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ConstructionLedgerService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<ClientLedgerEntryDto>> GetClientLedgerAsync(int clientId, int companyId)
        {
            var entries = await _context.ClientLedgerEntries
                .Where(e => e.ProjectClientId == clientId && e.CompanyId == companyId)
                .Include(e => e.ProjectClient)
                .Include(e => e.ConstructionProject)
                .OrderBy(e => e.EntryDate)
                .ToListAsync();

            return entries.Select(e =>
            {
                var dto = _mapper.Map<ClientLedgerEntryDto>(e);
                dto.ClientName = e.ProjectClient?.ClientName;
                dto.ProjectName = e.ConstructionProject?.ProjectName;
                return dto;
            }).ToList();
        }

        public async Task<List<InvestorLedgerEntryDto>> GetInvestorLedgerAsync(int investorId, int companyId)
        {
            var entries = await _context.InvestorLedgerEntries
                .Where(e => e.ProjectInvestorId == investorId && e.CompanyId == companyId)
                .Include(e => e.ProjectInvestor)
                .Include(e => e.ConstructionProject)
                .OrderBy(e => e.EntryDate)
                .ToListAsync();

            return entries.Select(e =>
            {
                var dto = _mapper.Map<InvestorLedgerEntryDto>(e);
                dto.InvestorName = e.ProjectInvestor?.InvestorName;
                dto.ProjectName = e.ConstructionProject?.ProjectName;
                return dto;
            }).ToList();
        }

        public async Task<List<ProjectLedgerEntryDto>> GetProjectLedgerAsync(int projectId, int companyId)
        {
            var entries = await _context.ProjectLedgerEntries
                .Where(e => e.ConstructionProjectId == projectId && e.CompanyId == companyId)
                .Include(e => e.ConstructionProject)
                .OrderBy(e => e.EntryDate)
                .ToListAsync();

            return entries.Select(e =>
            {
                var dto = _mapper.Map<ProjectLedgerEntryDto>(e);
                dto.ProjectName = e.ConstructionProject?.ProjectName;
                return dto;
            }).ToList();
        }
    }
}
