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
            });

            // Configure Product
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            });

            // Configure SaleOrder
            modelBuilder.Entity<SaleOrder>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.CustomerName).HasMaxLength(200);
                entity.Property(e => e.CustomerAddress).HasMaxLength(500);
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
                
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

            // Seed data
            modelBuilder.Entity<Customer>().HasData(
                new Customer { Id = 1, Name = "John Doe", Address = "123 Main St", Phone = "555-1234", Email = "john@example.com", IsActive = true },
                new Customer { Id = 2, Name = "Jane Smith", Address = "456 Oak Ave", Phone = "555-5678", Email = "jane@example.com", IsActive = true }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Product A", Description = "Description for Product A", Price = 10.50m, IsActive = true },
                new Product { Id = 2, Name = "Product B", Description = "Description for Product B", Price = 25.00m, IsActive = true },
                new Product { Id = 3, Name = "Product C", Description = "Description for Product C", Price = 15.75m, IsActive = true }
            );
        }
    }
}