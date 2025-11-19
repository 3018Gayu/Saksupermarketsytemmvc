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
    public class OrderDetailsController : Controller
    {
        private readonly SaksoftSupermarketSystemContext _context;

        public OrderDetailsController(SaksoftSupermarketSystemContext context)
        {
            _context = context;
        }

        // GET: OrderDetails
        public async Task<IActionResult> Index()
        {
            var orderDetails = _context.OrderDetails
                                       .Include(od => od.Order)
                                       .Include(od => od.Product);
            return View(await orderDetails.ToListAsync());
        }

        // GET: OrderDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var orderDetail = await _context.OrderDetails
                                            .Include(od => od.Order)
                                            .Include(od => od.Product)
                                            .FirstOrDefaultAsync(od => od.OrderDetailId == id);

            if (orderDetail == null) return NotFound();

            return View(orderDetail);
        }

        // GET: OrderDetails/Create
        public IActionResult Create(int? orderId = null)
        {
            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "InvoiceNo", orderId);
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "Name");
            return View();
        }

        // POST: OrderDetails/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,ProductId,Quantity,UnitPrice")] OrderDetails orderDetail)
        {
            if (ModelState.IsValid)
            {
                orderDetail.TotalPrice = orderDetail.Quantity * orderDetail.UnitPrice;

                // Reduce product stock
                var product = await _context.Products.FindAsync(orderDetail.ProductId);
                if (product != null)
                {
                    product.StockQty -= orderDetail.Quantity;
                }

                _context.OrderDetails.Add(orderDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Orders", new { id = orderDetail.OrderId });
            }

            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "InvoiceNo", orderDetail.OrderId);
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "Name", orderDetail.ProductId);
            return View(orderDetail);
        }

        // GET: OrderDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var orderDetail = await _context.OrderDetails.FindAsync(id);
            if (orderDetail == null) return NotFound();

            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "InvoiceNo", orderDetail.OrderId);
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "Name", orderDetail.ProductId);
            return View(orderDetail);
        }

        // POST: OrderDetails/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderDetailId,OrderId,ProductId,Quantity,UnitPrice,TotalPrice")] OrderDetails orderDetail)
        {
            if (id != orderDetail.OrderDetailId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    orderDetail.TotalPrice = orderDetail.Quantity * orderDetail.UnitPrice;
                    _context.Update(orderDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderDetailExists(orderDetail.OrderDetailId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "InvoiceNo", orderDetail.OrderId);
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "Name", orderDetail.ProductId);
            return View(orderDetail);
        }

        // GET: OrderDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var orderDetail = await _context.OrderDetails
                                            .Include(od => od.Order)
                                            .Include(od => od.Product)
                                            .FirstOrDefaultAsync(od => od.OrderDetailId == id);

            if (orderDetail == null) return NotFound();

            return View(orderDetail);
        }

        // POST: OrderDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orderDetail = await _context.OrderDetails.FindAsync(id);
            if (orderDetail != null)
            {
                _context.OrderDetails.Remove(orderDetail);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool OrderDetailExists(int id)
        {
            return _context.OrderDetails.Any(e => e.OrderDetailId == id);
        }
    }
}
