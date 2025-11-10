using Microsoft.AspNetCore.Mvc;
using Saksupermarketsytemmvc.web.Models;
using System.Linq;

namespace Saksupermarketsytemmvc.web.Controllers
{
    public class CategoryController : Controller
    {
        private readonly SaksoftSupermarketSystemContext _context;

        public CategoryController(SaksoftSupermarketSystemContext context)
        {
            _context = context;
        }

        // =============================
        // GET: Category/Index (Read)
        // =============================
        public IActionResult Index()
        {
            var categories = _context.Categories.ToList();

            // Display success messages from TempData
            if (TempData["SuccessMessage"] != null)
                ViewBag.SuccessMessage = TempData["SuccessMessage"];

            if (TempData["ErrorMessage"] != null)
                ViewBag.ErrorMessage = TempData["ErrorMessage"];

            return View(categories);
        }

        // =============================
        // GET: Category/Create
        // =============================
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // =============================
        // POST: Category/Create
        // =============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category model)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Add(model);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Category created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // =============================
        // GET: Category/Edit/{id}
        // =============================
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var category = _context.Categories.FirstOrDefault(x => x.CategoryId == id);
            if (category == null) return NotFound();
            return View(category);
        }

        // =============================
        // POST: Category/Edit
        // =============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category model)
        {
            if (ModelState.IsValid)
            {
                var category = _context.Categories.FirstOrDefault(x => x.CategoryId == model.CategoryId);
                if (category == null) return NotFound();

                category.CategoryName = model.CategoryName;
                category.Description = model.Description;

                _context.SaveChanges();
                TempData["SuccessMessage"] = "Category updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // =============================
        // GET: Category/Details/{id}
        // =============================
        public IActionResult Detail(int id)
        {
            var category = _context.Categories.FirstOrDefault(x => x.CategoryId == id);
            if (category == null) return NotFound();
            return View(category);
        }

        // =============================
        // GET: Category/Delete/{id}
        // =============================
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var category = _context.Categories.FirstOrDefault(x => x.CategoryId == id);
            if (category == null) return NotFound();

            // Show error message if any from TempData
            if (TempData["ErrorMessage"] != null)
                ViewBag.ErrorMessage = TempData["ErrorMessage"];

            return View(category);
        }

        // =============================
        // POST: Category/DeleteConfirmed
        // =============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int CategoryId)
        {
            var category = _context.Categories.FirstOrDefault(x => x.CategoryId == CategoryId);
            if (category != null)
            {
                // Check if related products exist for this category
                bool hasProducts = _context.Products.Any(p => p.CategoryId == CategoryId);
                if (hasProducts)
                {
                    TempData["ErrorMessage"] = "Cannot delete this category because it has associated products.";
                    return RedirectToAction(nameof(Delete), new { id = CategoryId });
                }

                _context.Categories.Remove(category);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Category deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
