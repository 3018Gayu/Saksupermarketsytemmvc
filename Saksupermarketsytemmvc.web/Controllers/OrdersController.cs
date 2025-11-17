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
    public class OrdersController : Controller
    {
        private readonly SaksoftSupermarketSystemContext _context;

        public OrdersController(SaksoftSupermarketSystemContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                                       .Include(o => o.Customer)
                                       .ToListAsync();
            return View(orders);
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var order = await _context.Orders
                                      .Include(o => o.Customer)
                                      .Include(o => o.OrderDetails)
                                      .ThenInclude(od => od.Product)
                                      .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null) return NotFound();
            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            LoadCustomers();
            return View();
        }

        // POST: Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Orders order)
        {
            if (ModelState.IsValid)
            {
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            LoadCustomers();
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            LoadCustomers();
            return View(order);
        }

        // POST: Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Orders order)
        {
            if (id != order.OrderId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Entry(order).Property(o => o.InvoiceNo).IsModified = false;
                    _context.Orders.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            LoadCustomers();
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id, string errorMessage = null)
        {
            if (id == null) return NotFound();

            var order = await _context.Orders
                                      .Include(o => o.Customer)
                                      .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null) return NotFound();

            if (!string.IsNullOrEmpty(errorMessage))
                ViewBag.ErrorMessage = errorMessage;

            return View(order);
        }

        // POST: Orders/Delete/5
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
                }
                catch (DbUpdateException)
                {
                    // Cannot delete due to FK constraint
                    string errorMessage = "Cannot delete this order because it has related order details.";
                    return RedirectToAction(nameof(Delete), new { id, errorMessage });
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // Helper to load customer dropdown
        private void LoadCustomers()
        {
            ViewBag.Customers = _context.Customers
                                        .Select(c => new SelectListItem
                                        {
                                            Value = c.CustomerId.ToString(),
                                            Text = c.CustomerName
                                        }).ToList();
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(o => o.OrderId == id);
        }
    }
}
