using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Saksupermarketsytemmvc.web.Models;
using System.Threading.Tasks;

namespace Saksupermarketsytemmvc.web.Controllers
{
    [Authorize(Roles = "Admin,Cashier,Inventory Manager")]
    public class ProductController : Controller
    {
        private readonly SaksoftSupermarketSystemContext _context;

        public ProductController(SaksoftSupermarketSystemContext context)
        {
            _context = context;
        }

        // ✅ GET: Product
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .ToListAsync();

            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.ErrorMessage = TempData["ErrorMessage"];
            return View(products);
        }

        // ✅ GET: Product/Create
        [HttpGet]
        public IActionResult Create()
        {
            // Only Admin or Inventory Manager can create products
            if (!User.IsInRole("Admin") && !User.IsInRole("Inventory Manager"))
                return Forbid();

            // 🟢 FIXED: Use "CategoryName" not "Name"
            ViewData["Categories"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");

            return View();
        }

        // ✅ POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (!User.IsInRole("Admin") && !User.IsInRole("Inventory Manager"))
                return Forbid();

            if (ModelState.IsValid)
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Product created successfully!";
                return RedirectToAction(nameof(Index));
            }

            // Rebind categories if ModelState fails
            ViewData["Categories"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        // ✅ GET: Product/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!User.IsInRole("Admin") && !User.IsInRole("Inventory Manager"))
                return Forbid();

            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            ViewData["Categories"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        // ✅ POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (!User.IsInRole("Admin") && !User.IsInRole("Inventory Manager"))
                return Forbid();

            if (id != product.ProductId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Product updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Products.Any(e => e.ProductId == id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["Categories"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        // ✅ GET: Product/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (!User.IsInRole("Admin") && !User.IsInRole("Inventory Manager"))
                return Forbid();

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        // ✅ POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!User.IsInRole("Admin") && !User.IsInRole("Inventory Manager"))
                return Forbid();

            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                try
                {
                    _context.Products.Remove(product);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Product deleted successfully!";
                }
                catch (DbUpdateException)
                {
                    TempData["ErrorMessage"] = "Cannot delete this product because it is referenced elsewhere.";
                    return RedirectToAction(nameof(Delete), new { id });
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
