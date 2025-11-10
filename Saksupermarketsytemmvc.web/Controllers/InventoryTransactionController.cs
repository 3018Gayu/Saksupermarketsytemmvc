using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Saksupermarketsytemmvc.web.Models;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementMVC.Controllers
{
    public class InventoryTransactionController : Controller
    {
        private readonly SaksoftSupermarketSystemContext _context;

        public InventoryTransactionController(SaksoftSupermarketSystemContext context)
        {
            _context = context;
        }

        // ===============================
        // READ: Index
        // ===============================
        public async Task<IActionResult> Index()
        {
            var transactions = _context.InventoryTransactions
                .Include(t => t.Product); // Include product for display
            return View(await transactions.ToListAsync());
        }

        // ===============================
        // CREATE: GET
        // ===============================
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Products = _context.Products.ToList(); // Populate dropdown
            return View();
        }

        // ===============================
        // CREATE: POST
        // ===============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InventoryTransaction model)
        {
            if (ModelState.IsValid)
            {
                _context.InventoryTransactions.Add(model);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Transaction created successfully!";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Products = _context.Products.ToList(); // Repopulate if validation fails
            return View(model);
        }

        // ===============================
        // EDIT: GET
        // ===============================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var transaction = await _context.InventoryTransactions.FindAsync(id);
            if (transaction == null) return NotFound();

            ViewBag.Products = _context.Products.ToList(); // Populate dropdown
            return View(transaction);
        }

        // ===============================
        // EDIT: POST
        // ===============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(InventoryTransaction model)
        {
            if (ModelState.IsValid)
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Transaction updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Products = _context.Products.ToList(); // Repopulate if validation fails
            return View(model);
        }

        // ===============================
        // DETAILS: GET
        // ===============================
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var transaction = await _context.InventoryTransactions
                .Include(t => t.Product)
                .FirstOrDefaultAsync(t => t.TransId == id);

            if (transaction == null) return NotFound();
            return View(transaction);
        }

        // ===============================
        // DELETE: GET
        // ===============================
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var transaction = await _context.InventoryTransactions
                .Include(t => t.Product)
                .FirstOrDefaultAsync(t => t.TransId == id);

            if (transaction == null) return NotFound();
            return View(transaction);
        }

        // ===============================
        // DELETE: POST
        // ===============================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transaction = await _context.InventoryTransactions.FindAsync(id);
            if (transaction != null)
            {
                _context.InventoryTransactions.Remove(transaction);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Transaction deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
