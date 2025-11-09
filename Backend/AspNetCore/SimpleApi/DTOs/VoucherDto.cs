namespace SimpleApi.DTOs
{
    public class VoucherDto
    {
        public int Id { get; set; }
        public string VoucherNumber { get; set; } = string.Empty;
        public string VoucherType { get; set; } = string.Empty;
        public DateTime VoucherDate { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? Description { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Status { get; set; }
        public bool IsPosted { get; set; }
        public DateTime? PostedDate { get; set; }
        public int? PostedByUserId { get; set; }
        public int CompanyId { get; set; }
        public List<JournalEntryDto> JournalEntries { get; set; } = new List<JournalEntryDto>();
    }

    public class CreateVoucherDto
    {
        public string VoucherNumber { get; set; } = string.Empty;
        public string VoucherType { get; set; } = string.Empty;
        public DateTime VoucherDate { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? Description { get; set; }
        public List<CreateJournalEntryDto> JournalEntries { get; set; } = new List<CreateJournalEntryDto>();
    }

    public class JournalEntryDto
    {
        public int Id { get; set; }
        public int VoucherId { get; set; }
        public int AccountId { get; set; }
        public string AccountName { get; set; } = string.Empty;
        public string EntryType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public DateTime EntryDate { get; set; }
    }

    public class CreateJournalEntryDto
    {
        public int AccountId { get; set; }
        public string EntryType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? Description { get; set; }
    }
}
