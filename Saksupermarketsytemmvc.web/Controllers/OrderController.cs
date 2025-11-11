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

        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders.Include(o => o.Customer)
                                              .ToListAsync();
            if (TempData["SuccessMessage"] != null)
                ViewBag.SuccessMessage = TempData["SuccessMessage"];
            if (TempData["ErrorMessage"] != null)
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders.Include(o => o.Customer)
                                             .Include(o => o.OrderDetails)
                                             .ThenInclude(od => od.Product)
                                             .FirstOrDefaultAsync(o => o.OrderId == id);
            if (order == null) return NotFound();
            return View(order);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "Name");
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "FullName");
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
            return View(order);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Order order)
        {
            if (id != order.OrderId) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(order);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Order updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _context.Orders.Include(o => o.Customer)
                                             .FirstOrDefaultAsync(o => o.OrderId == id);
            if (order == null) return NotFound();
            return View(order);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                try
                {
                    _context.Orders.Remove(order);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Order deleted successfully!";
                }
                catch (DbUpdateException)
                {
                    TempData["ErrorMessage"] = "Cannot delete this order because it is referenced elsewhere.";
                    return RedirectToAction(nameof(Delete), new { id });
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
