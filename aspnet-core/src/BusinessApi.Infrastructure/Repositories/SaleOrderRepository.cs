using BusinessApi.Domain.Entities;
using BusinessApi.Domain.Interfaces;
using BusinessApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BusinessApi.Infrastructure.Repositories
{
    public class SaleOrderRepository : ISaleOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public SaleOrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<SaleOrder>> GetAllAsync(int skip = 0, int take = 10)
        {
            return await _context.SaleOrders
                .Include(so => so.Items)
                    .ThenInclude(item => item.Product)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<SaleOrder?> GetAsync(int id)
        {
            return await _context.SaleOrders
                .Include(so => so.Items)
                    .ThenInclude(item => item.Product)
                .FirstOrDefaultAsync(so => so.Id == id);
        }

        public async Task<SaleOrder> CreateAsync(SaleOrder saleOrder)
        {
            _context.SaleOrders.Add(saleOrder);
            await _context.SaveChangesAsync();
            return saleOrder;
        }

        public async Task<SaleOrder> UpdateAsync(SaleOrder saleOrder)
        {
            _context.SaleOrders.Update(saleOrder);
            await _context.SaveChangesAsync();
            return saleOrder;
        }

        public async Task DeleteAsync(int id)
        {
            var saleOrder = await _context.SaleOrders.FindAsync(id);
            if (saleOrder != null)
            {
                _context.SaleOrders.Remove(saleOrder);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetCountAsync()
        {
            return await _context.SaleOrders.CountAsync();
        }
    }
}