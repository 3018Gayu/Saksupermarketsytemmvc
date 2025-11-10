using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Saksupermarketsytemmvc.web.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Saksupermarketsytemmvc.web.Controllers
{
    public class ProductController : Controller
    {
        private readonly SaksoftSupermarketSystemContext _context;

        public ProductController(SaksoftSupermarketSystemContext context)
        {
            _context = context;
        }

        // Helper to load categories for dropdown
        private void LoadCategories(object selectedCategory = null)
        {
            ViewData["Categories"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", selectedCategory);
        }

        // GET: Product
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.Include(p => p.Category).ToListAsync();

            if (TempData["SuccessMessage"] != null)
                ViewBag.SuccessMessage = TempData["SuccessMessage"];
            if (TempData["ErrorMessage"] != null)
                ViewBag.ErrorMessage = TempData["ErrorMessage"];

            return View(products);
        }

        // GET: Product/Create
        [HttpGet]
        public IActionResult Create()
        {
            LoadCategories();
            return View();
        }

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Make sure EF Core doesn't try to insert the category
                    product.Category = null;
                    _context.Products.Add(product);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Product created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", $"Database error: {ex.InnerException?.Message ?? ex.Message}");
                }
            }

            LoadCategories(product.CategoryId);
            return View(product);
        }

        // GET: Product/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null) return NotFound();

            LoadCategories(product.CategoryId);
            return View(product);
        }

        // POST: Product/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.ProductId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Fetch the existing entity
                    var existingProduct = await _context.Products.FindAsync(id);
                    if (existingProduct == null) return NotFound();

                    // Update fields manually to avoid EF Core tracking issues
                    existingProduct.Name = product.Name;
                    existingProduct.CategoryId = product.CategoryId;
                    existingProduct.UnitPrice = product.UnitPrice;
                    existingProduct.StockQty = product.StockQty;
                    existingProduct.ExpiryDate = product.ExpiryDate;

                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Product updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Products.Any(e => e.ProductId == id)) return NotFound();
                    else throw;
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", $"Database error: {ex.InnerException?.Message ?? ex.Message}");
                }
            }

            LoadCategories(product.CategoryId);
            return View(product);
        }

        // GET: Product/Delete/{id}
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.Include(p => p.Category)
                                                 .FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null) return NotFound();

            if (TempData["ErrorMessage"] != null)
                ViewBag.ErrorMessage = TempData["ErrorMessage"];

            return View(product);
        }

        // POST: Product/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
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
                    TempData["ErrorMessage"] = "Cannot delete this product because it is referenced in other records.";
                    return RedirectToAction(nameof(Delete), new { id });
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
