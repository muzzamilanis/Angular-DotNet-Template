using System.ComponentModel.DataAnnotations;

namespace SimpleApi.Models
{
    public class Property
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string PropertyName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string PropertyType { get; set; } = string.Empty; // Land, Building, Commercial, Residential

        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(100)]
        public string? State { get; set; }

        [MaxLength(20)]
        public string? ZipCode { get; set; }

        public decimal TotalArea { get; set; } // in square feet or meters

        [MaxLength(20)]
        public string? AreaUnit { get; set; } = "sqft";

        // Structure Details
        public int NumberOfRooms { get; set; }
        public int NumberOfFloors { get; set; }
        public bool HasLawn { get; set; }
        public decimal? LawnArea { get; set; }
        public bool HasGallery { get; set; }
        public int NumberOfGalleries { get; set; }

        [MaxLength(2000)]
        public string? CustomStructureDetails { get; set; }

        [MaxLength(50)]
        public string? PropertyStatus { get; set; } = "Available"; // Available, Sold, Rented, Under Development

        public decimal CurrentValue { get; set; }

        public DateTime AcquisitionDate { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(500)]
        public string? ImageUrls { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Multi-tenant
        [Required]
        public int CompanyId { get; set; }

        // Navigation properties
        public virtual Company Company { get; set; } = null!;
        public virtual ICollection<PropertyPurchase> Purchases { get; set; } = new List<PropertyPurchase>();
        public virtual ICollection<PropertySale> Sales { get; set; } = new List<PropertySale>();
        public virtual ICollection<OwnershipTransfer> OwnershipTransfers { get; set; } = new List<OwnershipTransfer>();
    }
}
