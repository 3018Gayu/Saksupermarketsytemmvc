using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Saksupermarketsytemmvc.web.Models;

namespace Saksupermarketsytemmvc.web.Controllers
{
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
                                       .Include(o => o.OrderDetails)
                                       .ThenInclude(od => od.Product)
                                       .ToListAsync();

            foreach (var order in orders)
            {
                decimal total = order.OrderDetails.Sum(od => od.TotalPrice); // TotalPrice should be decimal
                order.TotalAmount = total;
                order.NetAmount = total + order.TaxAmount - order.Discount;
            }

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

            decimal totalAmount = order.OrderDetails.Sum(od => od.TotalPrice);
            order.TotalAmount = totalAmount;
            order.NetAmount = totalAmount + order.TaxAmount - order.Discount;

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewBag.Customers = new SelectList(_context.Customers, "CustomerId", "CustomerName");
            return View();
        }

        // POST: Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,OrderDate,TaxAmount,Discount")] Orders order)
        {
            if (ModelState.IsValid)
            {
                order.TotalAmount = 0m;
                order.NetAmount = 0m;

                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Customers = new SelectList(_context.Customers, "CustomerId", "CustomerName", order.CustomerId);
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            ViewBag.Customers = new SelectList(_context.Customers, "CustomerId", "CustomerName", order.CustomerId);
            return View(order);
        }

        // POST: Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,CustomerId,OrderDate,TaxAmount,Discount")] Orders order)
        {
            if (id != order.OrderId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingOrder = await _context.Orders
                                                      .Include(o => o.OrderDetails)
                                                      .FirstOrDefaultAsync(o => o.OrderId == id);

                    if (existingOrder != null)
                    {
                        existingOrder.CustomerId = order.CustomerId;
                        existingOrder.OrderDate = order.OrderDate;
                        existingOrder.TaxAmount = order.TaxAmount;
                        existingOrder.Discount = order.Discount;

                        decimal totalAmount = existingOrder.OrderDetails.Sum(od => od.TotalPrice);
                        existingOrder.TotalAmount = totalAmount;
                        existingOrder.NetAmount = totalAmount + existingOrder.TaxAmount - existingOrder.Discount;

                        _context.Update(existingOrder);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Orders.Any(e => e.OrderId == order.OrderId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Customers = new SelectList(_context.Customers, "CustomerId", "CustomerName", order.CustomerId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var order = await _context.Orders
                                      .Include(o => o.Customer)
                                      .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null) return NotFound();

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders
                                      .Include(o => o.OrderDetails)
                                      .FirstOrDefaultAsync(o => o.OrderId == id);
            if (order != null)
            {
                _context.OrderDetails.RemoveRange(order.OrderDetails);
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}
