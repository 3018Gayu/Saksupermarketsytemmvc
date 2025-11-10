using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Saksupermarketsytemmvc.web.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Saksupermarketsytemmvc.web.Controllers
{
    public class CustomerController : Controller
    {
        private readonly SaksoftSupermarketSystemContext _context;

        public CustomerController(SaksoftSupermarketSystemContext context)
        {
            _context = context;
        }

        // =============================
        // GET: Customer/Index
        // =============================
        public async Task<IActionResult> Index()
        {
            var customers = await _context.Customers.ToListAsync();

            if (TempData["SuccessMessage"] != null)
                ViewBag.SuccessMessage = TempData["SuccessMessage"];
            if (TempData["ErrorMessage"] != null)
                ViewBag.ErrorMessage = TempData["ErrorMessage"];

            return View(customers);
        }

        // =============================
        // GET: Customer/Details/{id}
        // =============================
        public async Task<IActionResult> Details(int id)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == id);
            if (customer == null) return NotFound();
            return View(customer);
        }

        // =============================
        // GET: Customer/Create
        // =============================
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // =============================
        // POST: Customer/Create
        // =============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Customer customer)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Customers.Add(customer);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Customer created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", $"Database error: {ex.InnerException?.Message ?? ex.Message}");
                }
            }
            return View(customer);
        }

        // =============================
        // GET: Customer/Edit/{id}
        // =============================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return NotFound();
            return View(customer);
        }

        // =============================
        // POST: Customer/Edit
        // =============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Customer customer)
        {
            if (id != customer.CustomerId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Customer updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", $"Database error: {ex.InnerException?.Message ?? ex.Message}");
                }
            }
            return View(customer);
        }

        // =============================
        // GET: Customer/Delete/{id}
        // =============================
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == id);
            if (customer == null) return NotFound();

            if (TempData["ErrorMessage"] != null)
                ViewBag.ErrorMessage = TempData["ErrorMessage"];

            return View(customer);
        }

        // =============================
        // POST: Customer/Delete
        // =============================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                try
                {
                    _context.Customers.Remove(customer);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Customer deleted successfully!";
                }
                catch (DbUpdateException)
                {
                    TempData["ErrorMessage"] = "Cannot delete this customer because related orders exist.";
                    return RedirectToAction(nameof(Delete), new { id });
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
