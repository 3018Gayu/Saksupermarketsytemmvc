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
        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products
                                        .Include(p => p.Category)
                                        .FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null) return NotFound();
            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            PopulateCategories();
            return View(new Products());
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Products product)
        {
            if (ModelState.IsValid)
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Product added successfully!";
                return RedirectToAction(nameof(Index));
            }

            PopulateCategories(product.CategoryId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            PopulateCategories(product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Products product)
        {
            if (id != product.ProductId) return NotFound();

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
                    if (!ProductExists(id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            PopulateCategories(product.CategoryId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products
                                        .Include(p => p.Category)
                                        .FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null) return NotFound();
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
                _context.InventoryTransactions.RemoveRange(_context.InventoryTransactions.Where(it => it.ProductId == id));
                _context.OrderDetails.RemoveRange(_context.OrderDetails.Where(od => od.ProductId == id));
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Product deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id) => _context.Products.Any(p => p.ProductId == id);

        private void PopulateCategories(object selectedCategory = null)
        {
            var categories = _context.Categories?.ToList() ?? new List<Category>();
            ViewBag.Categories = new SelectList(categories, "CategoryId", "CategoryName", selectedCategory);
        }
    }
}
