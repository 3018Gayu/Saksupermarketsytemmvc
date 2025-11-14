using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Saksupermarketsytemmvc.web.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Saksupermarketsytemmvc.web.Controllers
{
    [Authorize(Roles = "Admin,Cashier")]
    public class OrderController : Controller
    {
        private readonly SaksoftSupermarketSystemContext _context;
        public OrderController(SaksoftSupermarketSystemContext context) => _context = context;

        // ====== INDEX ======
        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders.Include(o => o.Customer).ToListAsync();

            if (TempData["SuccessMessage"] != null)
                ViewBag.SuccessMessage = TempData["SuccessMessage"];
            if (TempData["ErrorMessage"] != null)
                ViewBag.ErrorMessage = TempData["ErrorMessage"];

            return View(orders);
        }

        // ====== DETAILS ======
        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders
                                      .Include(o => o.Customer)
                                      .Include(o => o.OrderDetails)
                                      .ThenInclude(od => od.Product)
                                      .FirstOrDefaultAsync(o => o.OrderId == id);
            if (order == null)
                return NotFound();

            return View(order);
        }

        // ====== CREATE ======
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["Customers"] = _context.Customers.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Order created successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewData["Customers"] = _context.Customers.ToList();
            return View(order);
        }

        // ====== EDIT ======
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound();

            ViewData["Customers"] = _context.Customers.ToList();
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Order order)
        {
            if (id != order.OrderId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Order updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    ViewBag.ErrorMessage = "Unable to update order.";
                }
            }

            ViewData["Customers"] = _context.Customers.ToList();
            return View(order);
        }

        // ====== DELETE (GET) ======
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _context.Orders
                                      .Include(o => o.Customer)
                                      .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
                return NotFound();

            return View(order);
        }

        // ====== DELETE (POST) ======
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders
                                      .Include(o => o.Customer)
                                      .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
                return NotFound();

            try
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Order deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                // ✅ Show the same Delete view again, with error message (no redirect)
                ViewBag.ErrorMessage = "⚠️ Cannot delete this order because it’s referenced elsewhere (e.g., has order details).";
                return View("Delete", order);
            }
        }
    }
}
