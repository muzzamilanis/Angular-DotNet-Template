using System.Linq;
using AbpCompanyName.AbpProjectName.EntityFrameworkCore;
using AbpCompanyName.AbpProjectName.SaleOrders;

namespace AbpCompanyName.AbpProjectName.EntityFrameworkCore.Seed.Host
{
    public class DefaultProductsCreator
    {
        private readonly AbpProjectNameDbContext _context;

        public DefaultProductsCreator(AbpProjectNameDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateProducts();
        }

        private void CreateProducts()
        {
            if (!_context.Products.Any())
            {
                var products = new[]
                {
                    new Product
                    {
                        Name = "Laptop",
                        Description = "High-performance laptop computer",
                        Price = 1200.00m,
                        IsActive = true
                    },
                    new Product
                    {
                        Name = "Desktop PC",
                        Description = "Powerful desktop computer",
                        Price = 800.00m,
                        IsActive = true
                    },
                    new Product
                    {
                        Name = "Monitor",
                        Description = "24-inch LED monitor",
                        Price = 300.00m,
                        IsActive = true
                    },
                    new Product
                    {
                        Name = "Keyboard",
                        Description = "Mechanical keyboard",
                        Price = 100.00m,
                        IsActive = true
                    },
                    new Product
                    {
                        Name = "Mouse",
                        Description = "Wireless optical mouse",
                        Price = 50.00m,
                        IsActive = true
                    }
                };

                foreach (var product in products)
                {
                    _context.Products.Add(product);
                }

                _context.SaveChanges();
            }
        }
    }
}