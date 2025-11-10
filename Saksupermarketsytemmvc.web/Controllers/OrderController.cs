using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Saksupermarketsytemmvc.web.Models;
using System.Linq;

namespace Saksupermarketsytemmvc.web.Controllers
{
    public class OrderController : Controller
    {
        private readonly SaksoftSupermarketSystemContext _context;

        public OrderController(SaksoftSupermarketSystemContext context)
        {
            _context = context;
        }

        // GET: Order
        public IActionResult Index()
        {
            var orders = _context.Orders
                .Include(o => o.Customer) // eager load customer info
                .ToList();

            return View(orders);
        }

        // GET: Order/Details/5
        public IActionResult Details(int id)
        {
            var order = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                .FirstOrDefault(o => o.OrderId == id);

            if (order == null)
                return NotFound();

            return View(order);
        }

        // GET: Order/Create
        public IActionResult Create()
        {
            ViewData["Customers"] = _context.Customers.ToList();
            return View();
        }

        // POST: Order/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Order order)
        {
            if (ModelState.IsValid)
            {
                // Calculate NetAmount = TotalAmount + TaxAmount - Discount (optional)
                order.NetAmount = (order.TotalAmount ?? 0) + (order.TaxAmount ?? 0) - (order.Discount ?? 0);

                _context.Orders.Add(order);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            ViewData["Customers"] = _context.Customers.ToList();
            return View(order);
        }

        // GET: Order/Edit/5
        public IActionResult Edit(int id)
        {
            var order = _context.Orders.Find(id);
            if (order == null)
                return NotFound();

            ViewData["Customers"] = _context.Customers.ToList();
            return View(order);
        }

        // POST: Order/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Order order)
        {
            if (id != order.OrderId)
                return BadRequest();

            if (ModelState.IsValid)
            {
                var existingOrder = _context.Orders.Find(id);
                if (existingOrder == null)
                    return NotFound();

                existingOrder.InvoiceNo = order.InvoiceNo;
                existingOrder.CustomerId = order.CustomerId;
                existingOrder.OrderDate = order.OrderDate;
                existingOrder.TotalAmount = order.TotalAmount;
                existingOrder.TaxAmount = order.TaxAmount;
                existingOrder.Discount = order.Discount;
                existingOrder.NetAmount = (order.TotalAmount ?? 0) + (order.TaxAmount ?? 0) - (order.Discount ?? 0);

                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            ViewData["Customers"] = _context.Customers.ToList();
            return View(order);
        }

        // GET: Order/Delete/5
        public IActionResult Delete(int id)
        {
            var order = _context.Orders
                .Include(o => o.Customer)
                .FirstOrDefault(o => o.OrderId == id);

            if (order == null)
                return NotFound();

            return View(order);
        }

        // POST: Order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var order = _context.Orders.Find(id);
            if (order == null)
                return NotFound();

            _context.Orders.Remove(order);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
