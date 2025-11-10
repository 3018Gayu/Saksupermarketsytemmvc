using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Saksupermarketsytemmvc.web.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Saksupermarketsytemmvc.web.Controllers
{
    public class SupplierController : Controller
    {
        private readonly SaksoftSupermarketSystemContext _context;

        public SupplierController(SaksoftSupermarketSystemContext context)
        {
            _context = context;
        }

        // GET: Supplier
        public async Task<IActionResult> Index()
        {
            var suppliers = await _context.Suppliers.ToListAsync();

            if (TempData["SuccessMessage"] != null)
                ViewBag.SuccessMessage = TempData["SuccessMessage"];
            if (TempData["ErrorMessage"] != null)
                ViewBag.ErrorMessage = TempData["ErrorMessage"];

            return View(suppliers);
        }

        // GET: Supplier/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            var supplier = await _context.Suppliers.FirstOrDefaultAsync(s => s.SupplierId == id);
            if (supplier == null)
                return NotFound();

            return View(supplier);
        }

        // GET: Supplier/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Supplier/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                _context.Suppliers.Add(supplier);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Supplier created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(supplier);
        }

        // GET: Supplier/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
                return NotFound();

            return View(supplier);
        }

        // POST: Supplier/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Supplier supplier)
        {
            if (id != supplier.SupplierId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(supplier);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Supplier updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", $"Database error: {ex.InnerException?.Message ?? ex.Message}");
                }
            }
            return View(supplier);
        }

        // GET: Supplier/Delete/{id}
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var supplier = await _context.Suppliers.FirstOrDefaultAsync(s => s.SupplierId == id);
            if (supplier == null)
                return NotFound();

            if (TempData["ErrorMessage"] != null)
                ViewBag.ErrorMessage = TempData["ErrorMessage"];

            return View(supplier);
        }

        // POST: Supplier/DeleteConfirmed
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier != null)
            {
                try
                {
                    _context.Suppliers.Remove(supplier);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Supplier deleted successfully!";
                }
                catch (DbUpdateException)
                {
                    TempData["ErrorMessage"] = "Cannot delete this supplier because it is referenced in other records.";
                    return RedirectToAction(nameof(Delete), new { id });
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
