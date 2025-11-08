namespace SimpleApi.DTOs
{
    public class AccountDto
    {
        public int Id { get; set; }
        public string AccountCode { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public string? AccountCategory { get; set; }
        public int? ParentAccountId { get; set; }
        public int Level { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal CurrentBalance { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public int CompanyId { get; set; }
    }

    public class CreateAccountDto
    {
        public string AccountCode { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public string? AccountCategory { get; set; }
        public int? ParentAccountId { get; set; }
        public decimal OpeningBalance { get; set; }
        public string? Description { get; set; }
    }

    public class UpdateAccountDto : CreateAccountDto
    {
        public int Id { get; set; }
    }
}
