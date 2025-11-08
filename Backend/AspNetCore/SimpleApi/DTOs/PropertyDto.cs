namespace SimpleApi.DTOs
{
    public class PropertyDto
    {
        public int Id { get; set; }
        public string PropertyName { get; set; } = string.Empty;
        public string PropertyType { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public decimal TotalArea { get; set; }
        public string? AreaUnit { get; set; }
        public int NumberOfRooms { get; set; }
        public int NumberOfFloors { get; set; }
        public bool HasLawn { get; set; }
        public decimal? LawnArea { get; set; }
        public bool HasGallery { get; set; }
        public int NumberOfGalleries { get; set; }
        public string? CustomStructureDetails { get; set; }
        public string? PropertyStatus { get; set; }
        public decimal CurrentValue { get; set; }
        public DateTime AcquisitionDate { get; set; }
        public string? Description { get; set; }
        public string? ImageUrls { get; set; }
        public bool IsActive { get; set; }
        public int CompanyId { get; set; }
    }

    public class CreatePropertyDto
    {
        public string PropertyName { get; set; } = string.Empty;
        public string PropertyType { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public decimal TotalArea { get; set; }
        public string? AreaUnit { get; set; } = "sqft";
        public int NumberOfRooms { get; set; }
        public int NumberOfFloors { get; set; }
        public bool HasLawn { get; set; }
        public decimal? LawnArea { get; set; }
        public bool HasGallery { get; set; }
        public int NumberOfGalleries { get; set; }
        public string? CustomStructureDetails { get; set; }
        public string? PropertyStatus { get; set; } = "Available";
        public decimal CurrentValue { get; set; }
        public DateTime AcquisitionDate { get; set; }
        public string? Description { get; set; }
        public string? ImageUrls { get; set; }
    }

    public class UpdatePropertyDto : CreatePropertyDto
    {
        public int Id { get; set; }
    }
}
