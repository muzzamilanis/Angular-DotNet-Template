using Microsoft.EntityFrameworkCore;
using SimpleApi.Models;
using SimpleApi.Models.CRM;

namespace SimpleApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<SaleOrder> SaleOrders { get; set; }
        public DbSet<SaleOrderItem> SaleOrderItems { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<BusinessNature> BusinessNatures { get; set; }
        public DbSet<ServiceType> ServiceTypes { get; set; }

        // CRM DbSets
        public DbSet<Lead> Leads { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Deal> Deals { get; set; }
        public DbSet<CrmTask> CrmTasks { get; set; }
        public DbSet<CrmFormTemplate> CrmFormTemplates { get; set; }
        public DbSet<CrmFormSubmission> CrmFormSubmissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure BusinessNature
            modelBuilder.Entity<BusinessNature>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.DisplayName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(500);
            });

            // Configure Company
            modelBuilder.Entity<Company>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Address).HasMaxLength(500);
                entity.Property(e => e.Phone).HasMaxLength(50);
                entity.Property(e => e.Email).HasMaxLength(200);
                entity.Property(e => e.ContactPerson).HasMaxLength(100);
                entity.Property(e => e.BusinessNatureId).IsRequired();
                
                entity.HasOne(e => e.BusinessNature)
                      .WithMany(e => e.Companies)
                      .HasForeignKey(e => e.BusinessNatureId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Project
            modelBuilder.Entity<Project>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ProjectName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.ProjectDescription).HasMaxLength(1000);
                entity.Property(e => e.ProjectStatus).IsRequired();
                
                entity.HasOne(e => e.Company)
                      .WithMany(e => e.Projects)
                      .HasForeignKey(e => e.CompanyId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure ServiceType
            modelBuilder.Entity<ServiceType>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(200);
                
                entity.HasOne(e => e.Company)
                      .WithMany()
                      .HasForeignKey(e => e.CompanyId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
                entity.Property(e => e.PasswordHash).IsRequired();
                
                entity.HasOne(e => e.Company)
                      .WithMany()
                      .HasForeignKey(e => e.CompanyId)
                      .OnDelete(DeleteBehavior.Restrict);
                      
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Configure Customer
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Address).HasMaxLength(500);
                entity.Property(e => e.Phone).HasMaxLength(50);
                entity.Property(e => e.Email).HasMaxLength(200);
                
                entity.HasOne(e => e.Company)
                      .WithMany()
                      .HasForeignKey(e => e.CompanyId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Product
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                
                entity.HasOne(e => e.Company)
                      .WithMany()
                      .HasForeignKey(e => e.CompanyId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure SaleOrder
            modelBuilder.Entity<SaleOrder>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.CustomerName).HasMaxLength(200);
                entity.Property(e => e.CustomerAddress).HasMaxLength(500);
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.AmountReceived).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TotalPayment).HasColumnType("decimal(18,2)");
                
                // Digital marketing specific field configurations
                entity.Property(e => e.Code).HasMaxLength(50);
                entity.Property(e => e.ClientName).HasMaxLength(200);
                entity.Property(e => e.Status).HasMaxLength(50);
                entity.Property(e => e.PaymentStatus).HasMaxLength(50);
                entity.Property(e => e.ContactEmail).HasMaxLength(200);
                entity.Property(e => e.ContactPhone).HasMaxLength(50);
                entity.Property(e => e.AssignedTo).HasMaxLength(200);
                entity.Property(e => e.OrderDetails).HasMaxLength(1000);
                entity.Property(e => e.WordsOrPages).HasMaxLength(100);
                entity.Property(e => e.AcademicLevel).HasMaxLength(100);
                entity.Property(e => e.Country).HasMaxLength(100);
                
                entity.HasOne(e => e.Company)
                      .WithMany()
                      .HasForeignKey(e => e.CompanyId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Project)
                      .WithMany(e => e.SaleOrders)
                      .HasForeignKey(e => e.ProjectId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.ServiceType)
                      .WithMany()
                      .HasForeignKey(e => e.ServiceTypeId)
                      .OnDelete(DeleteBehavior.SetNull);
                
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
                
                // Digital marketing specific field configurations
                entity.Property(e => e.WorkTitle).HasMaxLength(200);
                entity.Property(e => e.PaperType).HasMaxLength(100);
                
                entity.HasOne(e => e.SaleOrder)
                      .WithMany(e => e.Items)
                      .HasForeignKey(e => e.SaleOrderId);
                      
                entity.HasOne(e => e.Product)
                      .WithMany()
                      .HasForeignKey(e => e.ProductId);
            });

            // Seed data - Create business natures first
            modelBuilder.Entity<BusinessNature>().HasData(
                new BusinessNature { Id = 1, Name = "DigitalMarketingAgency", DisplayName = "Digital Marketing Agency", Description = "Digital marketing and content creation services", IsActive = true, CreatedAt = DateTime.UtcNow },
                new BusinessNature { Id = 2, Name = "Automobile", DisplayName = "Automobile", Description = "Automotive industry and vehicle-related services", IsActive = true, CreatedAt = DateTime.UtcNow },
                new BusinessNature { Id = 3, Name = "General", DisplayName = "General", Description = "General business operations and services", IsActive = true, CreatedAt = DateTime.UtcNow },
                new BusinessNature { Id = 4, Name = "Retail", DisplayName = "Retail & E-commerce", Description = "Traditional retail and online commerce operations", IsActive = true, CreatedAt = DateTime.UtcNow },
                new BusinessNature { Id = 5, Name = "Manufacturing", DisplayName = "Manufacturing", Description = "Manufacturing and production businesses", IsActive = true, CreatedAt = DateTime.UtcNow }
            );

            // Seed data - Create companies with business nature references
            modelBuilder.Entity<Company>().HasData(
                new Company { Id = 1, Name = "Default Company", Address = "123 Business St", Phone = "555-0001", Email = "admin@defaultcompany.com", ContactPerson = "System Admin", BusinessNatureId = 3, IsActive = true, CreatedAt = DateTime.UtcNow },
                new Company { Id = 2, Name = "Digital Marketing Pro", Address = "456 Marketing Ave", Phone = "555-0002", Email = "admin@digitalmarketingpro.com", ContactPerson = "Marketing Manager", BusinessNatureId = 1, IsActive = true, CreatedAt = DateTime.UtcNow }
            );

            // Seed data for default admin user
            // Password "123qwe" hashed with SHA256
            var adminPasswordHash = Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes("123qwe")));
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Name = "System Administrator", Email = "admin@aspnetboilerplate.com", PasswordHash = adminPasswordHash, IsActive = true, IsSuperAdmin = true, CompanyId = 1, CreatedAt = DateTime.UtcNow }
            );

            // Seed data for Customers (with CompanyId)
            modelBuilder.Entity<Customer>().HasData(
                new Customer { Id = 1, Name = "John Doe", Address = "123 Main St", Phone = "555-1234", Email = "john@example.com", IsActive = true, CompanyId = 1 },
                new Customer { Id = 2, Name = "Jane Smith", Address = "456 Oak Ave", Phone = "555-5678", Email = "jane@example.com", IsActive = true, CompanyId = 1 }
            );

            // Seed data for Products (with CompanyId)
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Product A", Description = "Description for Product A", Price = 10.50m, IsActive = true, CompanyId = 1 },
                new Product { Id = 2, Name = "Product B", Description = "Description for Product B", Price = 25.00m, IsActive = true, CompanyId = 1 },
                new Product { Id = 3, Name = "Product C", Description = "Description for Product C", Price = 15.75m, IsActive = true, CompanyId = 1 }
            );

            // Seed data for Projects
            modelBuilder.Entity<Project>().HasData(
                new Project { Id = 1, ProjectName = "Website Development", ProjectDescription = "Corporate website development project", ProjectStatus = ProjectStatus.Active, CreatedDate = DateTime.UtcNow, UpdatedDate = DateTime.UtcNow, CompanyId = 1, IsActive = true },
                new Project { Id = 2, ProjectName = "SEO Campaign", ProjectDescription = "Search engine optimization campaign", ProjectStatus = ProjectStatus.Active, CreatedDate = DateTime.UtcNow, UpdatedDate = DateTime.UtcNow, CompanyId = 1, IsActive = true },
                new Project { Id = 3, ProjectName = "Social Media Marketing", ProjectDescription = "Comprehensive social media marketing strategy", ProjectStatus = ProjectStatus.Active, CreatedDate = DateTime.UtcNow, UpdatedDate = DateTime.UtcNow, CompanyId = 2, IsActive = true },
                new Project { Id = 4, ProjectName = "Content Creation Campaign", ProjectDescription = "Blog and content creation for digital presence", ProjectStatus = ProjectStatus.Completed, CreatedDate = DateTime.UtcNow, UpdatedDate = DateTime.UtcNow, CompanyId = 2, IsActive = true }
            );

            // Seed data for ServiceTypes
            modelBuilder.Entity<ServiceType>().HasData(
                new ServiceType { Id = 1, Name = "Content Writing", Description = "Blog posts, articles, and other written content", IsActive = true, CompanyId = 1, CreatedAt = DateTime.UtcNow },
                new ServiceType { Id = 2, Name = "Web Design", Description = "Website design and development services", IsActive = true, CompanyId = 1, CreatedAt = DateTime.UtcNow },
                new ServiceType { Id = 3, Name = "SEO Writing", Description = "Search engine optimized content creation", IsActive = true, CompanyId = 1, CreatedAt = DateTime.UtcNow },
                new ServiceType { Id = 4, Name = "Strategy Planning", Description = "Digital marketing strategy development", IsActive = true, CompanyId = 1, CreatedAt = DateTime.UtcNow },
                new ServiceType { Id = 5, Name = "Email Marketing", Description = "Email campaign development and management", IsActive = true, CompanyId = 1, CreatedAt = DateTime.UtcNow },
                new ServiceType { Id = 6, Name = "SEO", Description = "Search engine optimization services", IsActive = true, CompanyId = 2, CreatedAt = DateTime.UtcNow },
                new ServiceType { Id = 7, Name = "Writing Service", Description = "General writing and content creation", IsActive = true, CompanyId = 2, CreatedAt = DateTime.UtcNow }
            );

            // Configure CRM entities
            ConfigureCrmEntities(modelBuilder);
        }

        private void ConfigureCrmEntities(ModelBuilder modelBuilder)
        {
            // Configure Lead
            modelBuilder.Entity<Lead>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Phone).HasMaxLength(50);
                entity.Property(e => e.Source).HasMaxLength(200);
                entity.Property(e => e.Notes).HasMaxLength(500);
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.BusinessNatureId).IsRequired();
                entity.Property(e => e.CompanyId).IsRequired();

                entity.HasOne(e => e.BusinessNature)
                      .WithMany()
                      .HasForeignKey(e => e.BusinessNatureId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Company)
                      .WithMany()
                      .HasForeignKey(e => e.CompanyId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.Email);
            });

            // Configure Contact
            modelBuilder.Entity<Contact>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Phone).HasMaxLength(50);
                entity.Property(e => e.JobTitle).HasMaxLength(100);
                entity.Property(e => e.Notes).HasMaxLength(500);
                entity.Property(e => e.CompanyId).IsRequired();

                entity.HasOne(e => e.Lead)
                      .WithMany(e => e.Contacts)
                      .HasForeignKey(e => e.LeadId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.Company)
                      .WithMany()
                      .HasForeignKey(e => e.CompanyId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.Email);
            });

            // Configure Deal
            modelBuilder.Entity<Deal>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Value).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.CompanyId).IsRequired();

                entity.HasOne(e => e.Lead)
                      .WithMany(e => e.Deals)
                      .HasForeignKey(e => e.LeadId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.Contact)
                      .WithMany(e => e.Deals)
                      .HasForeignKey(e => e.ContactId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.Company)
                      .WithMany()
                      .HasForeignKey(e => e.CompanyId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure CrmTask
            modelBuilder.Entity<CrmTask>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.Priority).IsRequired();
                entity.Property(e => e.AssignedToUserId).IsRequired();
                entity.Property(e => e.CompanyId).IsRequired();

                entity.HasOne(e => e.AssignedToUser)
                      .WithMany()
                      .HasForeignKey(e => e.AssignedToUserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Lead)
                      .WithMany(e => e.Tasks)
                      .HasForeignKey(e => e.LeadId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.Contact)
                      .WithMany(e => e.Tasks)
                      .HasForeignKey(e => e.ContactId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.Deal)
                      .WithMany(e => e.Tasks)
                      .HasForeignKey(e => e.DealId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.Company)
                      .WithMany()
                      .HasForeignKey(e => e.CompanyId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure CrmFormTemplate
            modelBuilder.Entity<CrmFormTemplate>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.JsonSchema).IsRequired();
                entity.Property(e => e.BusinessNatureId).IsRequired();
                entity.Property(e => e.CompanyId).IsRequired();

                entity.HasOne(e => e.BusinessNature)
                      .WithMany()
                      .HasForeignKey(e => e.BusinessNatureId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Company)
                      .WithMany()
                      .HasForeignKey(e => e.CompanyId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure CrmFormSubmission
            modelBuilder.Entity<CrmFormSubmission>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FormTemplateId).IsRequired();
                entity.Property(e => e.SubmittedData).IsRequired();
                entity.Property(e => e.CompanyId).IsRequired();

                entity.HasOne(e => e.FormTemplate)
                      .WithMany(e => e.FormSubmissions)
                      .HasForeignKey(e => e.FormTemplateId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Lead)
                      .WithMany(e => e.FormSubmissions)
                      .HasForeignKey(e => e.LeadId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.Contact)
                      .WithMany(e => e.FormSubmissions)
                      .HasForeignKey(e => e.ContactId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.Company)
                      .WithMany()
                      .HasForeignKey(e => e.CompanyId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}