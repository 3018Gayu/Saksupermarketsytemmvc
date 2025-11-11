using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saksupermarketsytemmvc.web.Models;

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
            var vm = new DashboardViewModel
            {
                ProductCount = _context.Products.Count(),
                CategoryCount = _context.Categories.Count(),
                SupplierCount = _context.Suppliers.Count(),
                CustomerCount = _context.Customers.Count(),
                OrderCount = _context.Orders.Count(),
                UserCount = _context.Users.Count()
            };
            return View(vm);
        }
    }

    public class DashboardViewModel
    {
        public int ProductCount { get; set; }
        public int CategoryCount { get; set; }
        public int SupplierCount { get; set; }
        public int CustomerCount { get; set; }
        public int OrderCount { get; set; }
        public int UserCount { get; set; }
    }
}