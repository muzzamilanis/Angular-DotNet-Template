using System.Collections.Generic;
using System.Linq;
using AbpCompanyName.AbpProjectName.EntityFrameworkCore;
using AbpCompanyName.AbpProjectName.SaleOrders;
using Microsoft.EntityFrameworkCore;

namespace AbpCompanyName.AbpProjectName.EntityFrameworkCore.Seed.Host
{
    public class DefaultCustomersCreator
    {
        private readonly AbpProjectNameDbContext _context;

        public DefaultCustomersCreator(AbpProjectNameDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateDefaultCustomers();
        }

        private void CreateDefaultCustomers()
        {
            var customers = new List<Customer>
            {
                new Customer
                {
                    Name = "John Doe",
                    Address = "123 Main St, New York, NY 10001",
                    Phone = "+1 (555) 123-4567",
                    Email = "john.doe@example.com",
                    IsActive = true
                },
                new Customer
                {
                    Name = "Jane Smith",
                    Address = "456 Oak Ave, Los Angeles, CA 90210",
                    Phone = "+1 (555) 987-6543",
                    Email = "jane.smith@example.com",
                    IsActive = true
                },
                new Customer
                {
                    Name = "Bob Johnson",
                    Address = "789 Pine Rd, Chicago, IL 60601",
                    Phone = "+1 (555) 555-5555",
                    Email = "bob.johnson@example.com",
                    IsActive = true
                },
                new Customer
                {
                    Name = "Alice Brown",
                    Address = "321 Elm St, Houston, TX 77001",
                    Phone = "+1 (555) 111-2222",
                    Email = "alice.brown@example.com",
                    IsActive = true
                },
                new Customer
                {
                    Name = "Charlie Wilson",
                    Address = "654 Maple Dr, Phoenix, AZ 85001",
                    Phone = "+1 (555) 333-4444",
                    Email = "charlie.wilson@example.com",
                    IsActive = false
                }
            };

            foreach (var customer in customers)
            {
                if (!_context.Customers.Any(c => c.Name == customer.Name))
                {
                    _context.Customers.Add(customer);
                }
            }

            _context.SaveChanges();
        }
    }
}