using Microsoft.EntityFrameworkCore;
using SimpleApi.Models;

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Company
            modelBuilder.Entity<Company>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Address).HasMaxLength(500);
                entity.Property(e => e.Phone).HasMaxLength(50);
                entity.Property(e => e.Email).HasMaxLength(200);
                entity.Property(e => e.ContactPerson).HasMaxLength(100);
            });

            // Configure User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
                entity.Property(e => e.PasswordHash).IsRequired();
                
                entity.HasOne<Company>()
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
                
                entity.HasOne<Company>()
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
                
                entity.HasOne<Company>()
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
                
                entity.HasOne<Company>()
                      .WithMany()
                      .HasForeignKey(e => e.CompanyId)
                      .OnDelete(DeleteBehavior.Restrict);
                
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

            // Seed data - Create a default company first
            modelBuilder.Entity<Company>().HasData(
                new Company { Id = 1, Name = "Default Company", Address = "123 Business St", Phone = "555-0001", Email = "admin@defaultcompany.com", ContactPerson = "System Admin", IsActive = true, CreatedAt = DateTime.UtcNow }
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
        }
    }
}