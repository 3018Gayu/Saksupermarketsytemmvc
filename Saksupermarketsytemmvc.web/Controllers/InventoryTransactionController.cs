using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
                .FirstOrDefaultAsync(m => m.TransId == id);

            if (transaction == null) return NotFound();

            return View(transaction);
        }

        // GET: InventoryTransaction/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "Name");
            return View();
        }

        // POST: InventoryTransaction/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TransId,ProductId,Quantity,Type,Remarks,Date")] InventoryTransaction transaction)
        {
            if (ModelState.IsValid)
            {
                _context.Add(transaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "Name", transaction.ProductId);
            return View(transaction);
        }

        // GET: InventoryTransaction/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var transaction = await _context.InventoryTransactions.FindAsync(id);
            if (transaction == null) return NotFound();

            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "Name", transaction.ProductId);
            return View(transaction);
        }

        // POST: InventoryTransaction/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TransId,ProductId,Quantity,Type,Remarks,Date")] InventoryTransaction transaction)
        {
            if (id != transaction.TransId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(transaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InventoryTransactionExists(transaction.TransId))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "Name", transaction.ProductId);
            return View(transaction);
        }

        // GET: InventoryTransaction/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var transaction = await _context.InventoryTransactions
                .Include(t => t.Product)
                .FirstOrDefaultAsync(m => m.TransId == id);

            if (transaction == null) return NotFound();

            return View(transaction);
        }

        // POST: InventoryTransaction/Delete
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

        private bool InventoryTransactionExists(int id)
        {
            return _context.InventoryTransactions.Any(e => e.TransId == id);
        }
    }
}
