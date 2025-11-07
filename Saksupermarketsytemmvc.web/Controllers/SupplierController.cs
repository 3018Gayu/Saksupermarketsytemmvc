using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Saksupermarketsytemmvc.web.Models;

namespace Saksupermarketsytemmvc.web.Controllers
{
    public class SupplierController : Controller
    {
        private readonly SaksoftSupermarketSystemContext _context;

        public SupplierController(SaksoftSupermarketSystemContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Ensure we pass an actual list to the view. If the DbSet is null, return empty list.
            var list = _context?.Suppliers != null
                ? await _context.Suppliers.ToListAsync()
                : new List<Supplier>();
            return View(list);
        }
    }
}