using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Saksupermarketsytemmvc.web.Models;

namespace Saksupermarketsytemmvc.web.Controllers
{
    public class CustomerController : Controller
    {
        private readonly SaksoftSupermarketSystemContext _context;
        public CustomerController(SaksoftSupermarketSystemContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            // Ensure we pass an actual list to the view. If the DbSet is null, return empty list.
            var list = _context?.Customers != null
                ? await _context.Customers.ToListAsync()
                : new List<Customer>();
            return View(list);
        }
    }
}
