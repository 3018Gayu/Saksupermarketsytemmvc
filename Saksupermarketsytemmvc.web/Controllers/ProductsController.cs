using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Saksupermarketsytemmvc.web.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Saksupermarketsytemmvc.web.Controllers
{
    [Authorize(Roles = "Admin,Cashier,Inventory Manager")]
    public class ProductsController : Controller
    {
        private readonly SaksoftSupermarketSystemContext _context;

        public ProductsController(SaksoftSupermarketSystemContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                                         .Include(p => p.Category)
                                         .ToListAsync();
            return View(products);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                                        .Include(p => p.Category)
                                        .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null) return NotFound();

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            LoadCategories();
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Products product)
        {
            if (string.IsNullOrWhiteSpace(product.ImageUrl))
                ModelState.AddModelError("ImageUrl", "Image URL is required.");

            if (ModelState.IsValid)
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Product added successfully.";
                return RedirectToAction(nameof(Index));
            }

            LoadCategories();
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            LoadCategories();
            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Products products)
        {
            if (id != products.ProductId) return NotFound();

            if (string.IsNullOrWhiteSpace(products.ImageUrl))
                ModelState.AddModelError("ImageUrl", "Image URL is required.");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Products.Update(products);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Product updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(products.ProductId))
                        return NotFound();
                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            LoadCategories();
            return View(products);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id, string errorMessage = null)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                                        .Include(p => p.Category)
                                        .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null) return NotFound();

            if (!string.IsNullOrEmpty(errorMessage))
                ViewBag.ErrorMessage = errorMessage;

            return View(product);
        }

        // POST: Products/Delete/5
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
                    TempData["SuccessMessage"] = "Product deleted successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    // If FK constraint exists, show warning in Delete page
                    string errorMessage = "Cannot delete this product because it is linked with existing orders.";
                    return RedirectToAction(nameof(Delete), new { id, errorMessage });
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private void LoadCategories()
        {
            ViewBag.Categories = _context.Categories
                                         .Select(c => new SelectListItem
                                         {
                                             Value = c.CategoryId.ToString(),
                                             Text = c.CategoryName
                                         }).ToList();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
