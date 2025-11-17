using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Saksupermarketsytemmvc.web.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Saksupermarketsytemmvc.web.Controllers
{
    [Authorize(Roles = "Admin,Inventory Manager")]
    public class InventoryTransactionController : Controller
    {
        private readonly SaksoftSupermarketSystemContext _context;

        public InventoryTransactionController(SaksoftSupermarketSystemContext context)
        {
            _context = context;
        }

        // GET: InventoryTransaction
        public async Task<IActionResult> Index()
        {
            var transactions = await _context.InventoryTransactions
                                             .Include(t => t.Product)
                                             .ToListAsync();
            return View(transactions);
        }

        // GET: InventoryTransaction/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var transaction = await _context.InventoryTransactions
                                            .Include(t => t.Product)
                                            .FirstOrDefaultAsync(t => t.TransId == id);

            if (transaction == null) return NotFound();
            return View(transaction);
        }

        // GET: InventoryTransaction/Create
        public IActionResult Create()
        {
            LoadProducts();
            return View();
        }

        // POST: InventoryTransaction/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InventoryTransaction transaction)
        {
            if (ModelState.IsValid)
            {
                _context.InventoryTransactions.Add(transaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            LoadProducts();
            return View(transaction);
        }

        // GET: InventoryTransaction/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var transaction = await _context.InventoryTransactions.FindAsync(id);
            if (transaction == null) return NotFound();

            LoadProducts();
            return View(transaction);
        }

        // POST: InventoryTransaction/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, InventoryTransaction transaction)
        {
            if (id != transaction.TransId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.InventoryTransactions.Update(transaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransactionExists(transaction.TransId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            LoadProducts();
            return View(transaction);
        }

        // GET: InventoryTransaction/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var transaction = await _context.InventoryTransactions
                                            .Include(t => t.Product)
                                            .FirstOrDefaultAsync(t => t.TransId == id);

            if (transaction == null) return NotFound();
            return View(transaction);
        }

        // POST: InventoryTransaction/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transaction = await _context.InventoryTransactions.FindAsync(id);
            if (transaction != null)
            {
                _context.InventoryTransactions.Remove(transaction);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // Helper method to load product dropdown
        private void LoadProducts()
        {
            ViewBag.Products = _context.Products
                                       .Where(p => p.IsActive == true)
                                       .Select(p => new SelectListItem
                                       {
                                           Value = p.ProductId.ToString(),
                                           Text = p.Name
                                       }).ToList();
        }

        private bool TransactionExists(int id)
        {
            return _context.InventoryTransactions.Any(t => t.TransId == id);
        }
    }
}
