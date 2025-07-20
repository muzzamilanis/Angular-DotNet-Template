using Microsoft.EntityFrameworkCore;
using SimpleApi.Models;

namespace SimpleApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Existing entities
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<SaleOrder> SaleOrders { get; set; }
        public DbSet<SaleOrderItem> SaleOrderItems { get; set; }

        // Multi-tenant and industry entities
        public DbSet<User> Users { get; set; }
        public DbSet<BusinessNature> BusinessNatures { get; set; }
        public DbSet<CompanyProfile> CompanyProfiles { get; set; }
        public DbSet<IndustryModule> IndustryModules { get; set; }
        public DbSet<IndustryFormConfig> IndustryFormConfigs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Customer
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Address).HasMaxLength(500);
                entity.Property(e => e.Phone).HasMaxLength(50);
                entity.Property(e => e.Email).HasMaxLength(200);
                entity.HasIndex(e => e.TenantId); // Index for multi-tenant queries
            });

            // Configure Product
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.HasIndex(e => e.TenantId); // Index for multi-tenant queries
            });

            // Configure SaleOrder
            modelBuilder.Entity<SaleOrder>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.CustomerName).HasMaxLength(200);
                entity.Property(e => e.CustomerAddress).HasMaxLength(500);
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
                entity.HasIndex(e => e.TenantId); // Index for multi-tenant queries
                
                entity.HasMany(e => e.Items)
                      .WithOne(e => e.SaleOrder)
                      .HasForeignKey(e => e.SaleOrderId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure SaleOrderItem
            modelBuilder.Entity<SaleOrderItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                
                entity.HasOne(e => e.SaleOrder)
                      .WithMany(e => e.Items)
                      .HasForeignKey(e => e.SaleOrderId);
                      
                entity.HasOne(e => e.Product)
                      .WithMany()
                      .HasForeignKey(e => e.ProductId);
            });

            // Configure User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.FirstName).HasMaxLength(200);
                entity.Property(e => e.LastName).HasMaxLength(200);
                entity.Property(e => e.Phone).HasMaxLength(50);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.TenantId);
            });

            // Configure BusinessNature
            modelBuilder.Entity<BusinessNature>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.DisplayName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.HasIndex(e => e.Name).IsUnique();
            });

            // Configure CompanyProfile
            modelBuilder.Entity<CompanyProfile>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CompanyName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Address).HasMaxLength(500);
                entity.Property(e => e.Phone).HasMaxLength(50);
                entity.Property(e => e.Email).HasMaxLength(200);
                entity.HasIndex(e => e.TenantId).IsUnique(); // One company per tenant

                entity.HasOne(e => e.BusinessNature)
                      .WithMany(e => e.CompanyProfiles)
                      .HasForeignKey(e => e.BusinessNatureId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure IndustryModule
            modelBuilder.Entity<IndustryModule>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ModuleName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.DisplayName).HasMaxLength(200);

                entity.HasOne(e => e.BusinessNature)
                      .WithMany()
                      .HasForeignKey(e => e.BusinessNatureId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.BusinessNatureId, e.ModuleName }).IsUnique();
            });

            // Configure IndustryFormConfig
            modelBuilder.Entity<IndustryFormConfig>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FormType).IsRequired().HasMaxLength(100);
                entity.Property(e => e.FieldSchema).IsRequired();

                entity.HasOne(e => e.BusinessNature)
                      .WithMany()
                      .HasForeignKey(e => e.BusinessNatureId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.BusinessNatureId, e.FormType }).IsUnique();
            });

            // Seed data for BusinessNatures
            modelBuilder.Entity<BusinessNature>().HasData(
                new BusinessNature 
                { 
                    Id = 1, 
                    Name = "DigitalMarketing", 
                    DisplayName = "Digital Marketing Agency",
                    Description = "Digital marketing and advertising agencies",
                    ConfigSchema = "{\"dashboardWidgets\":[\"campaignMetrics\",\"clientBilling\",\"adSpend\"],\"forms\":{\"purchase\":[\"vendor\",\"campaignId\",\"adPlatform\",\"budget\"],\"sales\":[\"clientId\",\"serviceType\",\"deliverables\",\"roi\"]},\"reports\":[\"campaignROI\",\"clientProfitability\",\"monthlyRevenue\"]}",
                    IsActive = true,
                    CreatedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new BusinessNature 
                { 
                    Id = 2, 
                    Name = "Automobile", 
                    DisplayName = "Automobile Industry",
                    Description = "Automotive sales and service businesses",
                    ConfigSchema = "{\"dashboardWidgets\":[\"inventory\",\"salesVolume\",\"serviceRevenue\"],\"forms\":{\"purchase\":[\"supplier\",\"partNumber\",\"vehicleModel\",\"quantity\"],\"sales\":[\"customerId\",\"vehicleVin\",\"financing\",\"warranty\"]},\"reports\":[\"inventoryTurnover\",\"salesPerformance\",\"serviceAnalytics\"]}",
                    IsActive = true,
                    CreatedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            // Seed data for IndustryModules
            modelBuilder.Entity<IndustryModule>().HasData(
                // Digital Marketing Modules
                new IndustryModule
                {
                    Id = 1,
                    BusinessNatureId = 1,
                    ModuleName = "CampaignManagement",
                    DisplayName = "Campaign Management",
                    ModuleConfig = "{\"features\":[\"createCampaign\",\"trackMetrics\",\"budgetManagement\"],\"permissions\":[\"campaign.create\",\"campaign.view\",\"campaign.edit\"]}",
                    IsEnabled = true,
                    CreatedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new IndustryModule
                {
                    Id = 2,
                    BusinessNatureId = 1,
                    ModuleName = "ClientBilling",
                    DisplayName = "Client Billing",
                    ModuleConfig = "{\"features\":[\"invoiceGeneration\",\"paymentTracking\",\"roiReporting\"],\"permissions\":[\"billing.create\",\"billing.view\",\"billing.edit\"]}",
                    IsEnabled = true,
                    CreatedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                // Automobile Modules
                new IndustryModule
                {
                    Id = 3,
                    BusinessNatureId = 2,
                    ModuleName = "InventoryManagement",
                    DisplayName = "Inventory Management",
                    ModuleConfig = "{\"features\":[\"stockTracking\",\"partsCatalog\",\"supplierManagement\"],\"permissions\":[\"inventory.create\",\"inventory.view\",\"inventory.edit\"]}",
                    IsEnabled = true,
                    CreatedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new IndustryModule
                {
                    Id = 4,
                    BusinessNatureId = 2,
                    ModuleName = "VehicleSales",
                    DisplayName = "Vehicle Sales",
                    ModuleConfig = "{\"features\":[\"vehicleCatalog\",\"salesTracking\",\"financingOptions\"],\"permissions\":[\"sales.create\",\"sales.view\",\"sales.edit\"]}",
                    IsEnabled = true,
                    CreatedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            // Seed data for IndustryFormConfigs
            modelBuilder.Entity<IndustryFormConfig>().HasData(
                // Digital Marketing Form Configs
                new IndustryFormConfig
                {
                    Id = 1,
                    BusinessNatureId = 1,
                    FormType = "purchase",
                    FieldSchema = "{\"fields\":[{\"name\":\"vendor\",\"type\":\"text\",\"label\":\"Vendor\",\"required\":true},{\"name\":\"campaignId\",\"type\":\"text\",\"label\":\"Campaign ID\",\"required\":true},{\"name\":\"adPlatform\",\"type\":\"select\",\"label\":\"Ad Platform\",\"options\":[\"Google Ads\",\"Facebook Ads\",\"LinkedIn Ads\",\"Twitter Ads\"],\"required\":true},{\"name\":\"budget\",\"type\":\"number\",\"label\":\"Budget\",\"required\":true,\"min\":0}]}",
                    ValidationRules = "{\"vendor\":{\"minLength\":2,\"maxLength\":100},\"budget\":{\"min\":0,\"max\":1000000}}",
                    IsActive = true,
                    CreatedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new IndustryFormConfig
                {
                    Id = 2,
                    BusinessNatureId = 1,
                    FormType = "sales",
                    FieldSchema = "{\"fields\":[{\"name\":\"clientId\",\"type\":\"text\",\"label\":\"Client ID\",\"required\":true},{\"name\":\"serviceType\",\"type\":\"select\",\"label\":\"Service Type\",\"options\":[\"PPC Management\",\"Social Media Marketing\",\"SEO\",\"Content Marketing\",\"Email Marketing\"],\"required\":true},{\"name\":\"deliverables\",\"type\":\"textarea\",\"label\":\"Deliverables\",\"required\":true},{\"name\":\"roi\",\"type\":\"number\",\"label\":\"Expected ROI (%)\",\"required\":false,\"min\":0}]}",
                    ValidationRules = "{\"clientId\":{\"minLength\":3,\"maxLength\":50},\"deliverables\":{\"minLength\":10,\"maxLength\":1000}}",
                    IsActive = true,
                    CreatedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                // Automobile Form Configs
                new IndustryFormConfig
                {
                    Id = 3,
                    BusinessNatureId = 2,
                    FormType = "purchase",
                    FieldSchema = "{\"fields\":[{\"name\":\"supplier\",\"type\":\"text\",\"label\":\"Supplier\",\"required\":true},{\"name\":\"partNumber\",\"type\":\"text\",\"label\":\"Part Number\",\"required\":true},{\"name\":\"vehicleModel\",\"type\":\"text\",\"label\":\"Vehicle Model\",\"required\":true},{\"name\":\"quantity\",\"type\":\"number\",\"label\":\"Quantity\",\"required\":true,\"min\":1}]}",
                    ValidationRules = "{\"supplier\":{\"minLength\":2,\"maxLength\":100},\"partNumber\":{\"pattern\":\"^[A-Z0-9-]+$\"},\"quantity\":{\"min\":1,\"max\":10000}}",
                    IsActive = true,
                    CreatedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new IndustryFormConfig
                {
                    Id = 4,
                    BusinessNatureId = 2,
                    FormType = "sales",
                    FieldSchema = "{\"fields\":[{\"name\":\"customerId\",\"type\":\"text\",\"label\":\"Customer ID\",\"required\":true},{\"name\":\"vehicleVin\",\"type\":\"text\",\"label\":\"Vehicle VIN\",\"required\":true},{\"name\":\"financing\",\"type\":\"select\",\"label\":\"Financing\",\"options\":[\"Cash\",\"Bank Loan\",\"Dealer Financing\",\"Lease\"],\"required\":true},{\"name\":\"warranty\",\"type\":\"select\",\"label\":\"Warranty\",\"options\":[\"Basic\",\"Extended\",\"Premium\"],\"required\":false}]}",
                    ValidationRules = "{\"customerId\":{\"minLength\":3,\"maxLength\":50},\"vehicleVin\":{\"pattern\":\"^[A-HJ-NPR-Z0-9]{17}$\",\"message\":\"Invalid VIN format\"}}",
                    IsActive = true,
                    CreatedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            // Seed data for existing entities (updated with TenantId)
            modelBuilder.Entity<Customer>().HasData(
                new Customer { Id = 1, TenantId = null, Name = "John Doe", Address = "123 Main St", Phone = "555-1234", Email = "john@example.com", IsActive = true, CreatedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Customer { Id = 2, TenantId = null, Name = "Jane Smith", Address = "456 Oak Ave", Phone = "555-5678", Email = "jane@example.com", IsActive = true, CreatedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, TenantId = null, Name = "Product A", Description = "Description for Product A", Price = 10.50m, IsActive = true, CreatedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Product { Id = 2, TenantId = null, Name = "Product B", Description = "Description for Product B", Price = 25.00m, IsActive = true, CreatedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Product { Id = 3, TenantId = null, Name = "Product C", Description = "Description for Product C", Price = 15.75m, IsActive = true, CreatedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );
        }
    }
}