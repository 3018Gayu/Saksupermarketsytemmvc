using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Saksupermarketsytemmvc.web.Models;

namespace Saksupermarketsytemmvc.web.Controllers
{
    public class InventoryTransactionController : Controller
    {
        private readonly SaksoftSupermarketSystemContext _context;
        public InventoryTransactionController(SaksoftSupermarketSystemContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Ensure we pass an actual list to the view. If the DbSet is null, return empty list.
            var list = _context?.InventoryTransactions != null
                ? await _context.InventoryTransactions.ToListAsync()
                : new List<InventoryTransaction>();
            return View(list);
        }
    }
}
