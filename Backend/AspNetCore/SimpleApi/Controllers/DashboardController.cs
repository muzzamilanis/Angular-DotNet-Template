using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleApi.Data;

namespace SimpleApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var companyId = GetCurrentUserCompanyId();
            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);

            var orders = await _context.SaleOrders
                .Where(o => o.CompanyId == companyId)
                .ToListAsync();

            var ordersThisMonth = orders.Where(o => o.OrderDate >= startOfMonth).ToList();

            // KPI counts
            var totalOrdersThisMonth = ordersThisMonth.Count;
            var pendingOrders = orders.Count(o => o.Status == "New" || o.Status == "InProgress" || o.Status == "Pending");
            var deliveredOrders = orders.Count(o => o.Status == "Delivered");
            var revenueThisMonth = ordersThisMonth
                .Sum(o => o.AmountReceivedInPkr ?? o.AmountReceived ?? 0);

            // Orders by status breakdown (all-time, for chart)
            var byStatus = orders
                .GroupBy(o => string.IsNullOrWhiteSpace(o.Status) ? "Unknown" : o.Status)
                .Select(g => new { status = g.Key, count = g.Count() })
                .OrderByDescending(x => x.count)
                .ToList();

            // Recent 6 orders
            var recentOrders = orders
                .OrderByDescending(o => o.OrderDate)
                .Take(6)
                .Select(o => new
                {
                    o.Id,
                    o.OrderNumber,
                    clientName = o.ClientName ?? o.CustomerName ?? "—",
                    o.Status,
                    o.OrderDate,
                    o.PaymentStatus
                })
                .ToList();

            return Ok(new
            {
                totalOrdersThisMonth,
                pendingOrders,
                deliveredOrders,
                revenueThisMonth,
                byStatus,
                recentOrders
            });
        }
    }
}
