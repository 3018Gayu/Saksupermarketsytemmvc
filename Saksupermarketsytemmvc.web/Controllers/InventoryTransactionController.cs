using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Saksupermarketsytemmvc.web.Models;
using System.Threading.Tasks;

namespace Saksupermarketsytemmvc.web.Controllers
{
    [Authorize(Roles = "Admin,Inventory Manager")]
    public class InventoryTransactionController : Controller
    {
        private readonly SaksoftSupermarketSystemContext _context;
        public InventoryTransactionController(SaksoftSupermarketSystemContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var transactions = await _context.InventoryTransactions
                                             .Include(t => t.Product)
                                             .ToListAsync();
            return View(transactions);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InventoryTransaction transaction)
        {
            if (ModelState.IsValid)
            {
                _context.InventoryTransactions.Add(transaction);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Transaction added successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(transaction);
        }
    }
}
