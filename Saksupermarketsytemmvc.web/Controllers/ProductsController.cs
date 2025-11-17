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
            ViewBag.Categories = _context.Categories
                                         .Select(c => new SelectListItem
                                         {
                                             Value = c.CategoryId.ToString(),
                                             Text = c.CategoryName
                                         }).ToList();
            return View();
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
                TempData["SuccessMessage"] = "Product added successfully.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = _context.Categories
                                         .Select(c => new SelectListItem
                                         {
                                             Value = c.CategoryId.ToString(),
                                             Text = c.CategoryName
                                         }).ToList();
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            ViewBag.Categories = _context.Categories
                                         .Select(c => new SelectListItem
                                         {
                                             Value = c.CategoryId.ToString(),
                                             Text = c.CategoryName
                                         }).ToList();
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
                    _context.Products.Update(product);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Product updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = _context.Categories
                                         .Select(c => new SelectListItem
                                         {
                                             Value = c.CategoryId.ToString(),
                                             Text = c.CategoryName
                                         }).ToList();
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

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
                try
                {
                    _context.Products.Remove(product);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Product deleted successfully.";
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException != null && ex.InnerException.Message.Contains("REFERENCE constraint"))
                    {
                        TempData["ErrorMessage"] = "Cannot delete this product. It is linked with existing orders.";
                        return RedirectToAction(nameof(Index));
                    }
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
