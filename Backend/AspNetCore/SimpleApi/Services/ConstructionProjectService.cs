using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SimpleApi.Data;
using SimpleApi.DTOs;
using SimpleApi.Models;

namespace SimpleApi.Services
{
    public class ConstructionProjectService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ConstructionProjectService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedResultDto<ConstructionProjectDto>> GetAllAsync(PagedResultRequestDto input, int companyId)
        {
            var query = _context.ConstructionProjects
                .Where(p => p.CompanyId == companyId)
                .Include(p => p.ProjectClients)
                .Include(p => p.ProjectInvestors)
                .Include(p => p.ProjectPayments)
                .Include(p => p.ProjectExpenses)
                .AsQueryable();

            if (!string.IsNullOrEmpty(input.Filter))
            {
                query = query.Where(p => p.ProjectName.Contains(input.Filter) ||
                                        (p.ProjectDescription != null && p.ProjectDescription.Contains(input.Filter)) ||
                                        (p.Location != null && p.Location.Contains(input.Filter)));
            }

            if (!string.IsNullOrEmpty(input.Sorting))
            {
                query = input.Sorting.ToLower() switch
                {
                    "projectname" => query.OrderBy(p => p.ProjectName),
                    "projectname desc" => query.OrderByDescending(p => p.ProjectName),
                    "status" => query.OrderBy(p => p.Status),
                    "status desc" => query.OrderByDescending(p => p.Status),
                    "createdat" => query.OrderBy(p => p.CreatedAt),
                    "createdat desc" => query.OrderByDescending(p => p.CreatedAt),
                    _ => query.OrderByDescending(p => p.CreatedAt)
                };
            }
            else
            {
                query = query.OrderByDescending(p => p.CreatedAt);
            }

            var totalCount = await query.CountAsync();

            var projects = await query
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .Select(p => new ConstructionProjectDto
                {
                    Id = p.Id,
                    ProjectName = p.ProjectName,
                    ProjectDescription = p.ProjectDescription,
                    Location = p.Location,
                    Status = p.Status,
                    EstimatedBudget = p.EstimatedBudget,
                    ActualCost = p.ActualCost,
                    StartDate = p.StartDate,
                    ExpectedEndDate = p.ExpectedEndDate,
                    ActualEndDate = p.ActualEndDate,
                    ProjectManager = p.ProjectManager,
                    Contractor = p.Contractor,
                    IsActive = p.IsActive,
                    CreatedAt = p.CreatedAt,
                    CompanyId = p.CompanyId,
                    ClientsCount = p.ProjectClients.Count,
                    InvestorsCount = p.ProjectInvestors.Count,
                    TotalPayments = p.ProjectPayments.Where(x => x.IsActive).Sum(x => x.Amount),
                    TotalExpenses = p.ProjectExpenses.Where(x => x.IsActive).Sum(x => x.Amount),
                    ProfitOrLoss = p.ProjectPayments.Where(x => x.IsActive).Sum(x => x.Amount) -
                                   p.ProjectExpenses.Where(x => x.IsActive).Sum(x => x.Amount)
                })
                .ToListAsync();

            return new PagedResultDto<ConstructionProjectDto>(totalCount, projects);
        }

        public async Task<ConstructionProjectDto> GetAsync(int id, int companyId)
        {
            var project = await _context.ConstructionProjects
                .Include(p => p.Company)
                .Include(p => p.ProjectClients)
                .Include(p => p.ProjectInvestors)
                .Include(p => p.ProjectPayments)
                .Include(p => p.ProjectExpenses)
                .FirstOrDefaultAsync(p => p.Id == id && p.CompanyId == companyId);

            if (project == null)
                throw new ArgumentException($"Construction project with ID {id} not found or access denied.");

            var projectDto = _mapper.Map<ConstructionProjectDto>(project);
            projectDto.CompanyName = project.Company?.Name;
            projectDto.ClientsCount = project.ProjectClients.Count;
            projectDto.InvestorsCount = project.ProjectInvestors.Count;
            projectDto.TotalPayments = project.ProjectPayments.Where(x => x.IsActive).Sum(x => x.Amount);
            projectDto.TotalExpenses = project.ProjectExpenses.Where(x => x.IsActive).Sum(x => x.Amount);
            projectDto.ProfitOrLoss = projectDto.TotalPayments - projectDto.TotalExpenses;

            return projectDto;
        }

        public async Task<ConstructionProjectDto> CreateAsync(CreateConstructionProjectDto input, int companyId)
        {
            var project = _mapper.Map<ConstructionProject>(input);
            project.CompanyId = companyId;
            project.CreatedAt = DateTime.UtcNow;
            project.ActualCost = 0;

            _context.ConstructionProjects.Add(project);
            await _context.SaveChangesAsync();

            return await GetAsync(project.Id, companyId);
        }

        public async Task<ConstructionProjectDto> UpdateAsync(UpdateConstructionProjectDto input, int companyId)
        {
            var project = await _context.ConstructionProjects
                .FirstOrDefaultAsync(p => p.Id == input.Id && p.CompanyId == companyId);

            if (project == null)
                throw new ArgumentException($"Construction project with ID {input.Id} not found or access denied.");

            _mapper.Map(input, project);
            project.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return await GetAsync(project.Id, companyId);
        }

        public async Task DeleteAsync(int id, int companyId)
        {
            var project = await _context.ConstructionProjects
                .Include(p => p.ProjectClients)
                .Include(p => p.ProjectInvestors)
                .Include(p => p.ProjectPayments)
                .Include(p => p.ProjectExpenses)
                .FirstOrDefaultAsync(p => p.Id == id && p.CompanyId == companyId);

            if (project == null)
                throw new ArgumentException($"Construction project with ID {id} not found or access denied.");

            // Soft delete if has related data
            if (project.ProjectClients.Any() || project.ProjectInvestors.Any() ||
                project.ProjectPayments.Any() || project.ProjectExpenses.Any())
            {
                project.IsActive = false;
                project.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                _context.ConstructionProjects.Remove(project);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<ConstructionDashboardDto> GetDashboardDataAsync(int companyId)
        {
            var projects = await _context.ConstructionProjects
                .Where(p => p.CompanyId == companyId && p.IsActive)
                .Include(p => p.ProjectPayments)
                .Include(p => p.ProjectExpenses)
                .ToListAsync();

            var recentPayments = await _context.ProjectPayments
                .Where(p => p.CompanyId == companyId && p.IsActive)
                .OrderByDescending(p => p.PaymentDate)
                .Take(5)
                .Include(p => p.ConstructionProject)
                .Include(p => p.ProjectClient)
                .Include(p => p.ProjectInvestor)
                .Select(p => _mapper.Map<ProjectPaymentDto>(p))
                .ToListAsync();

            var recentExpenses = await _context.ProjectExpenses
                .Where(p => p.CompanyId == companyId && p.IsActive)
                .OrderByDescending(p => p.ExpenseDate)
                .Take(5)
                .Include(p => p.ConstructionProject)
                .Select(p => _mapper.Map<ProjectExpenseDto>(p))
                .ToListAsync();

            var activeProjects = projects.Where(p => p.Status == ConstructionProjectStatus.InProgress).ToList();

            var dashboard = new ConstructionDashboardDto
            {
                TotalProjects = projects.Count,
                ActiveProjects = projects.Count(p => p.Status == ConstructionProjectStatus.InProgress),
                CompletedProjects = projects.Count(p => p.Status == ConstructionProjectStatus.Completed),
                OnHoldProjects = projects.Count(p => p.Status == ConstructionProjectStatus.OnHold),
                TotalEstimatedBudget = projects.Sum(p => p.EstimatedBudget),
                TotalActualCost = projects.Sum(p => p.ActualCost),
                TotalPaymentsReceived = projects.Sum(p => p.ProjectPayments.Where(x => x.IsActive).Sum(x => x.Amount)),
                TotalExpenses = projects.Sum(p => p.ProjectExpenses.Where(x => x.IsActive).Sum(x => x.Amount)),
                TotalClients = await _context.ProjectClients.Where(c => c.CompanyId == companyId && c.IsActive).CountAsync(),
                TotalInvestors = await _context.ProjectInvestors.Where(i => i.CompanyId == companyId && i.IsActive).CountAsync(),
                RecentPayments = recentPayments,
                RecentExpenses = recentExpenses,
                ActiveProjectsList = activeProjects.Take(5).Select(p => _mapper.Map<ConstructionProjectDto>(p)).ToList()
            };

            dashboard.OverallProfitOrLoss = dashboard.TotalPaymentsReceived - dashboard.TotalExpenses;

            return dashboard;
        }

        public async Task<ProjectProfitLossDto> GetProjectProfitLossAsync(int projectId, int companyId)
        {
            var project = await _context.ConstructionProjects
                .Include(p => p.ProjectPayments)
                .Include(p => p.ProjectExpenses)
                .FirstOrDefaultAsync(p => p.Id == projectId && p.CompanyId == companyId);

            if (project == null)
                throw new ArgumentException($"Construction project with ID {projectId} not found or access denied.");

            var totalRevenue = project.ProjectPayments.Where(x => x.IsActive).Sum(x => x.Amount);
            var totalExpenses = project.ProjectExpenses.Where(x => x.IsActive).Sum(x => x.Amount);
            var profitOrLoss = totalRevenue - totalExpenses;

            var expensesByCategory = project.ProjectExpenses
                .Where(x => x.IsActive)
                .GroupBy(x => x.ExpenseCategory)
                .Select(g => new ExpenseCategoryDto
                {
                    Category = g.Key,
                    Amount = g.Sum(x => x.Amount),
                    Percentage = totalExpenses > 0 ? (g.Sum(x => x.Amount) / totalExpenses * 100) : 0
                })
                .OrderByDescending(x => x.Amount)
                .ToList();

            return new ProjectProfitLossDto
            {
                ProjectId = project.Id,
                ProjectName = project.ProjectName,
                TotalRevenue = totalRevenue,
                TotalExpenses = totalExpenses,
                ProfitOrLoss = profitOrLoss,
                ProfitMargin = totalRevenue > 0 ? (profitOrLoss / totalRevenue * 100) : 0,
                ExpenseBreakdown = expensesByCategory
            };
        }
    }
}
