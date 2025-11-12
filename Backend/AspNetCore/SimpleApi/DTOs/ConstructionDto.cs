using System.ComponentModel.DataAnnotations;
using SimpleApi.Models;

namespace SimpleApi.DTOs
{
    public class ConstructionProjectDto
    {
        public int Id { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string? ProjectDescription { get; set; }
        public string? Location { get; set; }
        public ConstructionProjectStatus Status { get; set; }
        public decimal EstimatedBudget { get; set; }
        public decimal ActualCost { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ExpectedEndDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public string? ProjectManager { get; set; }
        public string? Contractor { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CompanyId { get; set; }
        public string? CompanyName { get; set; }
        
        // Summary fields
        public int ClientsCount { get; set; }
        public int InvestorsCount { get; set; }
        public decimal TotalPayments { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal ProfitOrLoss { get; set; }
    }

    public class CreateConstructionProjectDto
    {
        [Required]
        [StringLength(200)]
        public string ProjectName { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? ProjectDescription { get; set; }

        [StringLength(500)]
        public string? Location { get; set; }

        public ConstructionProjectStatus Status { get; set; } = ConstructionProjectStatus.Planning;

        [Range(0, double.MaxValue)]
        public decimal EstimatedBudget { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? ExpectedEndDate { get; set; }

        [StringLength(100)]
        public string? ProjectManager { get; set; }

        [StringLength(100)]
        public string? Contractor { get; set; }
    }

    public class UpdateConstructionProjectDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string ProjectName { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? ProjectDescription { get; set; }

        [StringLength(500)]
        public string? Location { get; set; }

        public ConstructionProjectStatus Status { get; set; }

        [Range(0, double.MaxValue)]
        public decimal EstimatedBudget { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? ExpectedEndDate { get; set; }

        public DateTime? ActualEndDate { get; set; }

        [StringLength(100)]
        public string? ProjectManager { get; set; }

        [StringLength(100)]
        public string? Contractor { get; set; }

        public bool IsActive { get; set; }
    }

    public class ProjectClientDto
    {
        public int Id { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? ContactPerson { get; set; }
        public string? CnicOrNtn { get; set; }
        public decimal SharePercentage { get; set; }
        public bool IsActive { get; set; }
        public int ConstructionProjectId { get; set; }
        public string? ProjectName { get; set; }
        public decimal LedgerBalance { get; set; }
    }

    public class CreateProjectClientDto
    {
        [Required]
        [StringLength(200)]
        public string ClientName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(50)]
        public string? Phone { get; set; }

        [StringLength(200)]
        [EmailAddress]
        public string? Email { get; set; }

        [StringLength(100)]
        public string? ContactPerson { get; set; }

        [StringLength(100)]
        public string? CnicOrNtn { get; set; }

        [Range(0, 100)]
        public decimal SharePercentage { get; set; }

        [Required]
        public int ConstructionProjectId { get; set; }
    }

    public class UpdateProjectClientDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string ClientName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(50)]
        public string? Phone { get; set; }

        [StringLength(200)]
        [EmailAddress]
        public string? Email { get; set; }

        [StringLength(100)]
        public string? ContactPerson { get; set; }

        [StringLength(100)]
        public string? CnicOrNtn { get; set; }

        [Range(0, 100)]
        public decimal SharePercentage { get; set; }

        public bool IsActive { get; set; }
    }

    public class ProjectInvestorDto
    {
        public int Id { get; set; }
        public string InvestorName { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? ContactPerson { get; set; }
        public string? CnicOrNtn { get; set; }
        public decimal InvestmentAmount { get; set; }
        public decimal SharePercentage { get; set; }
        public bool IsActive { get; set; }
        public int ConstructionProjectId { get; set; }
        public string? ProjectName { get; set; }
        public decimal LedgerBalance { get; set; }
    }

    public class CreateProjectInvestorDto
    {
        [Required]
        [StringLength(200)]
        public string InvestorName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(50)]
        public string? Phone { get; set; }

        [StringLength(200)]
        [EmailAddress]
        public string? Email { get; set; }

        [StringLength(100)]
        public string? ContactPerson { get; set; }

        [StringLength(100)]
        public string? CnicOrNtn { get; set; }

        [Range(0, double.MaxValue)]
        public decimal InvestmentAmount { get; set; }

        [Range(0, 100)]
        public decimal SharePercentage { get; set; }

        [Required]
        public int ConstructionProjectId { get; set; }
    }

    public class UpdateProjectInvestorDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string InvestorName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(50)]
        public string? Phone { get; set; }

        [StringLength(200)]
        [EmailAddress]
        public string? Email { get; set; }

        [StringLength(100)]
        public string? ContactPerson { get; set; }

        [StringLength(100)]
        public string? CnicOrNtn { get; set; }

        [Range(0, double.MaxValue)]
        public decimal InvestmentAmount { get; set; }

        [Range(0, 100)]
        public decimal SharePercentage { get; set; }

        public bool IsActive { get; set; }
    }

    public class ProjectPaymentDto
    {
        public int Id { get; set; }
        public string PaymentNumber { get; set; } = string.Empty;
        public PaymentSource PaymentSource { get; set; }
        public int? ProjectClientId { get; set; }
        public string? ClientName { get; set; }
        public int? ProjectInvestorId { get; set; }
        public string? InvestorName { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? PaymentMethod { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public int ConstructionProjectId { get; set; }
        public string? ProjectName { get; set; }
        public int? VoucherId { get; set; }
    }

    public class CreateProjectPaymentDto
    {
        [Required]
        public PaymentSource PaymentSource { get; set; }

        public int? ProjectClientId { get; set; }

        public int? ProjectInvestorId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; }

        [StringLength(50)]
        public string? PaymentMethod { get; set; }

        [StringLength(100)]
        public string? ReferenceNumber { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        public int ConstructionProjectId { get; set; }
    }

    public class ProjectExpenseDto
    {
        public int Id { get; set; }
        public string ExpenseNumber { get; set; } = string.Empty;
        public string ExpenseCategory { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string? VendorName { get; set; }
        public string? InvoiceNumber { get; set; }
        public string? PaymentMethod { get; set; }
        public string? ReferenceNumber { get; set; }
        public bool IsActive { get; set; }
        public int ConstructionProjectId { get; set; }
        public string? ProjectName { get; set; }
        public int? VoucherId { get; set; }
    }

    public class CreateProjectExpenseDto
    {
        [Required]
        [StringLength(200)]
        public string ExpenseCategory { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        public DateTime ExpenseDate { get; set; }

        [StringLength(100)]
        public string? VendorName { get; set; }

        [StringLength(100)]
        public string? InvoiceNumber { get; set; }

        [StringLength(50)]
        public string? PaymentMethod { get; set; }

        [StringLength(100)]
        public string? ReferenceNumber { get; set; }

        [Required]
        public int ConstructionProjectId { get; set; }
    }

    public class ClientLedgerEntryDto
    {
        public int Id { get; set; }
        public string EntryNumber { get; set; } = string.Empty;
        public DateTime EntryDate { get; set; }
        public string EntryType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public string? Description { get; set; }
        public string? ReferenceNumber { get; set; }
        public int ProjectClientId { get; set; }
        public string? ClientName { get; set; }
        public int ConstructionProjectId { get; set; }
        public string? ProjectName { get; set; }
    }

    public class InvestorLedgerEntryDto
    {
        public int Id { get; set; }
        public string EntryNumber { get; set; } = string.Empty;
        public DateTime EntryDate { get; set; }
        public string EntryType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public string? Description { get; set; }
        public string? ReferenceNumber { get; set; }
        public int ProjectInvestorId { get; set; }
        public string? InvestorName { get; set; }
        public int ConstructionProjectId { get; set; }
        public string? ProjectName { get; set; }
    }

    public class ProjectLedgerEntryDto
    {
        public int Id { get; set; }
        public string EntryNumber { get; set; } = string.Empty;
        public DateTime EntryDate { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public string EntryType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public string? Description { get; set; }
        public string? ReferenceNumber { get; set; }
        public int ConstructionProjectId { get; set; }
        public string? ProjectName { get; set; }
    }

    public class ConstructionDashboardDto
    {
        public int TotalProjects { get; set; }
        public int ActiveProjects { get; set; }
        public int CompletedProjects { get; set; }
        public int OnHoldProjects { get; set; }
        public decimal TotalEstimatedBudget { get; set; }
        public decimal TotalActualCost { get; set; }
        public decimal TotalPaymentsReceived { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal OverallProfitOrLoss { get; set; }
        public int TotalClients { get; set; }
        public int TotalInvestors { get; set; }
        public List<ProjectPaymentDto>? RecentPayments { get; set; }
        public List<ProjectExpenseDto>? RecentExpenses { get; set; }
        public List<ConstructionProjectDto>? ActiveProjectsList { get; set; }
    }

    public class ProjectProfitLossDto
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public decimal TotalRevenue { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal ProfitOrLoss { get; set; }
        public decimal ProfitMargin { get; set; }
        public List<ExpenseCategoryDto>? ExpenseBreakdown { get; set; }
    }

    public class ExpenseCategoryDto
    {
        public string Category { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal Percentage { get; set; }
    }
}
