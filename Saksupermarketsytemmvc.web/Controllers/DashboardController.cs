using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saksupermarketsytemmvc.web.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Collections.Generic;

namespace Saksupermarketsytemmvc.web.Controllers
{
    [Authorize(Roles = "Admin,Cashier,Inventory Manager")]
    public class DashboardController : Controller
    {
        private readonly SaksoftSupermarketSystemContext _context;

        public DashboardController(SaksoftSupermarketSystemContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "";
            var vm = new Dashboard();

            // ---------------- ROLE BASED STATS --------------------
            if (userRole.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                vm.ProductsCount = _context.Products.Count();
                vm.CategoryCount = _context.Categories.Count();
                vm.SupplierCount = _context.Suppliers.Count();
                vm.CustomerCount = _context.Customers.Count();
                vm.UserCount = _context.Users.Count();

                vm.LowStockProducts = _context.Products
                    .Where(p => p.StockQty <= p.MinimumStockLevel)
                    .ToList();

                vm.ExpiredProducts = _context.Products
                    .Where(p => p.ExpiryDate != null && p.ExpiryDate <= DateTime.Today)
                    .ToList();
            }
            else if (userRole.Equals("Cashier", StringComparison.OrdinalIgnoreCase))
            {
                vm.CustomerCount = _context.Customers.Count();
            }
            else if (userRole.Equals("Inventory Manager", StringComparison.OrdinalIgnoreCase))
            {
                vm.ProductsCount = _context.Products.Count();
                vm.CategoryCount = _context.Categories.Count();
                vm.SupplierCount = _context.Suppliers.Count();

                vm.LowStockProducts = _context.Products
                    .Where(p => p.StockQty <= p.MinimumStockLevel)
                    .ToList();

                vm.ExpiredProducts = _context.Products
                    .Where(p => p.ExpiryDate != null && p.ExpiryDate <= DateTime.Today)
                    .ToList();
            }

            // ---------------- LAST 7 DAYS SALES USING BILLS --------------------
            var today = DateTime.Today;

            var last7Days = Enumerable.Range(0, 7)
                .Select(i => today.AddDays(-i))
                .OrderBy(d => d)
                .ToList();

            vm.Last7DaysLabels = last7Days.Select(d => d.ToString("dd MMM")).ToList();

            // get all bills in range once
            var billsInRange = _context.Bills
                .Where(b => last7Days.Contains(b.BillDate.Date))
                .ToList();

            // group bills by date and sum amounts
            var salesByDate = billsInRange
                .GroupBy(b => b.BillDate.Date)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(x => x.TotalAmount)
                );

            vm.Last7DaysSales = last7Days
                .Select(d => salesByDate.ContainsKey(d) ? salesByDate[d] : 0m)
                .ToList();

            ViewData["UserRole"] = userRole;
            return View(vm);
        }
    }
}
