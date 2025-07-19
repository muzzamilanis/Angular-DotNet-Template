using Microsoft.EntityFrameworkCore;
using BusinessApi.Infrastructure.Data;
using BusinessApi.Domain.Entities;

namespace BusinessApi.Tests
{
    public class DatabaseConnectivityTests
    {
        [Fact]
        public void ApplicationDbContext_CanBeCreated_WithInMemoryDatabase()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            // Act & Assert
            using var context = new ApplicationDbContext(options);
            Assert.NotNull(context);
            Assert.NotNull(context.Customers);
            Assert.NotNull(context.Products);
            Assert.NotNull(context.SaleOrders);
            Assert.NotNull(context.SaleOrderItems);
        }

        [Fact]
        public void ApplicationDbContext_SeedData_IsConfiguredCorrectly()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "SeedTestDatabase")
                .Options;

            // Act
            using var context = new ApplicationDbContext(options);
            context.Database.EnsureCreated();

            // Assert
            var customers = context.Customers.ToList();
            var products = context.Products.ToList();

            Assert.Equal(2, customers.Count);
            Assert.Equal(3, products.Count);
            
            Assert.Contains(customers, c => c.Name == "John Doe");
            Assert.Contains(customers, c => c.Name == "Jane Smith");
            
            Assert.Contains(products, p => p.Name == "Product A");
            Assert.Contains(products, p => p.Name == "Product B");
            Assert.Contains(products, p => p.Name == "Product C");
        }
    }
}