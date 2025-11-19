using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saksupermarketsytemmvc.web.Models;
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
            var vm = new DashboardViewModel();

            // Admin can see all
            if (userRole.Equals("Admin", System.StringComparison.OrdinalIgnoreCase))
            {
                vm.ProductsCount = _context.Products.Count();
                vm.CategoryCount = _context.Categories.Count();
                vm.SupplierCount = _context.Suppliers.Count();
                vm.CustomerCount = _context.Customers.Count();
                vm.OrdersCount = _context.Orders.Count();
                vm.UserCount = _context.Users.Count();

                // Low stock products for Admin
                vm.LowStockProducts = _context.Products
                    .Where(p => p.StockQty <= p.MinimumStockLevel)
                    .ToList();
            }
            // Cashier can see only Orders, Customers
            else if (userRole.Equals("Cashier", System.StringComparison.OrdinalIgnoreCase))
            {
                vm.CustomerCount = _context.Customers.Count();
                vm.OrdersCount = _context.Orders.Count();
            }
            // Inventory Manager can see Products, Categories, Suppliers
            else if (userRole.Equals("Inventory Manager", System.StringComparison.OrdinalIgnoreCase))
            {
                vm.ProductsCount = _context.Products.Count();
                vm.CategoryCount = _context.Categories.Count();
                vm.SupplierCount = _context.Suppliers.Count();

                // Low stock products for Inventory Manager
                vm.LowStockProducts = _context.Products
                    .Where(p => p.StockQty <= p.MinimumStockLevel)
                    .ToList();
            }

            ViewData["UserRole"] = userRole;
            return View(vm);
        }
    }

    public class DashboardViewModel
    {
        public int ProductsCount { get; set; }
        public int CategoryCount { get; set; }
        public int SupplierCount { get; set; }
        public int CustomerCount { get; set; }
        public int OrdersCount { get; set; }
        public int UserCount { get; set; }

        // Add this back to avoid Razor view errors
        public List<Products> LowStockProducts { get; set; } = new List<Products>();
    }
}
