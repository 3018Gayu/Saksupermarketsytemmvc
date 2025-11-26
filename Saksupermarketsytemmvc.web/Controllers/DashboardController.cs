using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Saksupermarketsytemmvc.web.Models;

namespace Saksupermarketsytemmvc.web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly SaksoftSupermarketSystemContext _context;

        public DashboardController(SaksoftSupermarketSystemContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var dashboard = new Dashboard();
            var today = DateTime.Today;

            // ---------------- ADMIN ----------------
            if (User.IsInRole("Admin"))
            {
                dashboard.TotalSales = await _context.Bills.SumAsync(b => b.TotalAmount);
                dashboard.CustomerCount = await _context.Customers.CountAsync();
                dashboard.LowStockCount = await _context.Products.CountAsync(p => (p.StockQty ?? 0) < 10);
                dashboard.UserCount = await _context.Users.CountAsync();

                dashboard.RecentTransactions = await _context.Bills
                    .Include(b => b.Customer)
                    .OrderByDescending(b => b.BillDate)
                    .Take(10)
                    .Select(b => new RecentTransactionDTO
                    {
                        Date = b.BillDate.ToString("dd/MM/yy"),
                        InvoiceNo = b.BillId.ToString(),
                        Customer = b.Customer != null ? b.Customer.CustomerName : "Walk-in",
                        Amount = b.TotalAmount,
                    }).ToListAsync();

                var salesData = await _context.Bills
                    .Where(b => b.BillDate >= today.AddDays(-6))
                    .GroupBy(b => b.BillDate.Date)
                    .Select(g => new { Date = g.Key, TotalAmount = g.Sum(x => x.TotalAmount) })
                    .OrderBy(d => d.Date)
                    .ToListAsync();

                dashboard.Last7DaysLabels = salesData.Select(s => s.Date.ToString("dd MMM")).ToList();
                dashboard.Last7DaysSales = salesData.Select(s => s.TotalAmount).ToList();
            }

            // ---------------- CASHIER ----------------
            if (User.IsInRole("Cashier"))
            {
                dashboard.TodaysSales = await _context.Bills
                    .Where(b => b.BillDate.Date == today)
                    .SumAsync(b => b.TotalAmount);

                dashboard.TodaysInvoices = await _context.Bills
                    .Where(b => b.BillDate.Date == today)
                    .CountAsync();

                dashboard.RecentBills = await _context.Bills
                    .Where(b => b.BillDate.Date == today)
                    .Include(b => b.Customer)
                    .OrderByDescending(b => b.BillDate)
                    .Take(10)
                    .Select(b => new RecentTransactionDTO
                    {
                        Date = b.BillDate.ToString("HH:mm"),
                        InvoiceNo = b.BillId.ToString(),
                        Customer = b.Customer != null ? b.Customer.CustomerName : "Walk-in",
                        Amount = b.TotalAmount
                    }).ToListAsync();
            }

            // ---------------- INVENTORY MANAGER ----------------
            if (User.IsInRole("Inventory Manager"))
            {
                dashboard.ProductsCount = await _context.Products.CountAsync();
                dashboard.SupplierCount = await _context.Suppliers.CountAsync();

                // Low Stock
                dashboard.LowStockProducts = await _context.Products
                    .Where(p => (p.StockQty ?? 0) < 10)
                    .OrderBy(p => p.StockQty)
                    .ToListAsync();

                // Expired Products
                dashboard.ExpiredProducts = await _context.Products
                    .Where(p => p.ExpiryDate.HasValue && p.ExpiryDate.Value < today)
                    .OrderBy(p => p.ExpiryDate)
                    .ToListAsync();

                dashboard.RecentProducts = await _context.Products
                    .OrderByDescending(p => p.ProductId)
                    .Take(10)
                    .ToListAsync();

                // Fix: Set orders and order details counts
                dashboard.OrdersCount = await _context.Orders.CountAsync();
                dashboard.OrderDetailsCount = await _context.OrderDetails.CountAsync();
            }

            return View(dashboard);
        }
    }
}
