namespace SimpleApi.DTOs
{
    public class WriterWorkloadDto
    {
        public int WriterId { get; set; }
        public string WriterName { get; set; } = string.Empty;
        public int TotalAssignments { get; set; }
        public int ActiveAssignments { get; set; }
        public int CompletedAssignments { get; set; }
        public int PendingAssignments { get; set; }
        public List<WriterAssignmentDto> CurrentAssignments { get; set; } = new();
        public bool HasCapacity { get; set; }
        public int MaxCapacity { get; set; }
    }
}