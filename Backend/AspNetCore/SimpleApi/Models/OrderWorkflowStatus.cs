namespace SimpleApi.Models
{
	public class OrderWorkflowStatus
	{
		public int Id { get; set; }
		public int OrderId { get; set; }
		public string Stage { get; set; } = string.Empty;       // "OrderReceived", "AIProcessing", "WriterAssigned", "InProgress", "UnderReview", "Delivered"
		public string? AssignedTo { get; set; }                 // Writer name
		public string UpdatedBy { get; set; } = string.Empty;  // "System" or username
		public DateTime Timestamp { get; set; }
		public string? Notes { get; set; }

		// Navigation property
		public SaleOrder? SaleOrder { get; set; }
	}
}
