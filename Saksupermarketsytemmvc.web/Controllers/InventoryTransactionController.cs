using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Saksupermarketsytemmvc.web.Models;
using System.Threading.Tasks;
using System.Linq;

namespace Saksupermarketsytemmvc.web.Controllers
{
    [Authorize(Roles = "Admin,Inventory Manager")]
    public class InventoryTransactionController : Controller
    {
        private readonly SaksoftSupermarketSystemContext _context;
        public InventoryTransactionController(SaksoftSupermarketSystemContext context) => _context = context;

        // INDEX
        public async Task<IActionResult> Index()
        {
            var transactions = await _context.InventoryTransactions
                                             .Include(t => t.Product)
                                             .OrderByDescending(t => t.TransDate)
                                             .ToListAsync();

            if (TempData["SuccessMessage"] != null)
                ViewBag.SuccessMessage = TempData["SuccessMessage"];
            if (TempData["ErrorMessage"] != null)
                ViewBag.ErrorMessage = TempData["ErrorMessage"];

            return View(transactions);
        }

        // DETAILS
        public async Task<IActionResult> Details(int id)
        {
            var transaction = await _context.InventoryTransactions
                                            .Include(t => t.Product)
                                            .FirstOrDefaultAsync(t => t.TransId == id);
            if (transaction == null) return NotFound();
            return View(transaction);
        }

        // CREATE
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["Products"] = new SelectList(_context.Products, "ProductId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InventoryTransaction transaction)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Products"] = new SelectList(_context.Products, "ProductId", "Name", transaction.ProductId);
                return View(transaction);
            }

            try
            {
                _context.InventoryTransactions.Add(transaction);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Transaction added successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ViewBag.ErrorMessage = "Error saving transaction. Please try again.";
                ViewData["Products"] = new SelectList(_context.Products, "ProductId", "Name", transaction.ProductId);
                return View(transaction);
            }
        }

        // EDIT
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var transaction = await _context.InventoryTransactions.FindAsync(id);
            if (transaction == null) return NotFound();

            ViewData["Products"] = new SelectList(_context.Products, "ProductId", "Name", transaction.ProductId);
            return View(transaction);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, InventoryTransaction transaction)
        {
            if (id != transaction.TransId) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["Products"] = new SelectList(_context.Products, "ProductId", "Name", transaction.ProductId);
                return View(transaction);
            }

            try
            {
                _context.Update(transaction);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Transaction updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ViewBag.ErrorMessage = "Error updating transaction. Please try again.";
                ViewData["Products"] = new SelectList(_context.Products, "ProductId", "Name", transaction.ProductId);
                return View(transaction);
            }
        }

        // DELETE
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var transaction = await _context.InventoryTransactions
                                            .Include(t => t.Product)
                                            .FirstOrDefaultAsync(t => t.TransId == id);
            if (transaction == null) return NotFound();

            if (TempData["ErrorMessage"] != null)
                ViewBag.ErrorMessage = TempData["ErrorMessage"];

            return View(transaction);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transaction = await _context.InventoryTransactions.FindAsync(id);
            if (transaction == null)
            {
                TempData["ErrorMessage"] = "Transaction not found or already deleted.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.InventoryTransactions.Remove(transaction);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Transaction deleted successfully!";
            }
            catch
            {
                TempData["ErrorMessage"] = "Cannot delete this transaction because it’s referenced elsewhere.";
                return RedirectToAction(nameof(Delete), new { id });
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
