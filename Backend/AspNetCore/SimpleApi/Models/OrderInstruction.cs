using System.ComponentModel.DataAnnotations;

namespace SimpleApi.Models
{
    public class OrderInstruction
    {
        public int Id { get; set; }

        [Required]
        public int SaleOrderId { get; set; }
        public virtual SaleOrder? SaleOrder { get; set; }

        [Required]
        public int SenderId { get; set; }
        public virtual User? Sender { get; set; }

        [Required]
        [StringLength(2000)]
        public string Message { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsFromTeamLead { get; set; }

        [StringLength(500)]
        public string? AttachmentFilenames { get; set; } // Comma-separated filenames for instruction attachments
    }
}