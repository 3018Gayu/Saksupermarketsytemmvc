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
    public class OrderDetailController : Controller
    {
        private readonly SaksoftSupermarketSystemContext _context;

        public OrderDetailController(SaksoftSupermarketSystemContext context)
        {
            _context = context;
        }

        // GET: OrderDetail
        public async Task<IActionResult> Index()
        {
            var orderDetails = await _context.OrderDetails
                                             .Include(od => od.Order)
                                             .Include(od => od.Product)
                                             .ToListAsync();
            return View(orderDetails);
        }

        // GET: OrderDetail/Details/5
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

        // GET: OrderDetail/Create
        public IActionResult Create()
        {
            ViewBag.Orders = _context.Orders
                                     .Select(o => new SelectListItem
                                     {
                                         Value = o.OrderId.ToString(),
                                         Text = o.InvoiceNo
                                     }).ToList();

            ViewBag.Products = _context.Products
                                       .Select(p => new SelectListItem
                                       {
                                           Value = p.ProductId.ToString(),
                                           Text = p.Name
                                       }).ToList();

            return View();
        }

        // POST: OrderDetail/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderDetail orderDetail)
        {
            if (ModelState.IsValid)
            {
                // Calculate TotalPrice
                orderDetail.TotalPrice = (orderDetail.Quantity ?? 0) * (orderDetail.UnitPrice ?? 0);

                _context.OrderDetails.Add(orderDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Orders = _context.Orders
                                     .Select(o => new SelectListItem
                                     {
                                         Value = o.OrderId.ToString(),
                                         Text = o.InvoiceNo
                                     }).ToList();

            ViewBag.Products = _context.Products
                                       .Select(p => new SelectListItem
                                       {
                                           Value = p.ProductId.ToString(),
                                           Text = p.Name
                                       }).ToList();

            return View(orderDetail);
        }

        // GET: OrderDetail/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var orderDetail = await _context.OrderDetails.FindAsync(id);
            if (orderDetail == null) return NotFound();

            ViewBag.Orders = _context.Orders
                                     .Select(o => new SelectListItem
                                     {
                                         Value = o.OrderId.ToString(),
                                         Text = o.InvoiceNo
                                     }).ToList();

            ViewBag.Products = _context.Products
                                       .Select(p => new SelectListItem
                                       {
                                           Value = p.ProductId.ToString(),
                                           Text = p.Name
                                       }).ToList();

            return View(orderDetail);
        }

        // POST: OrderDetail/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OrderDetail orderDetail)
        {
            if (id != orderDetail.OrderDetailId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Recalculate TotalPrice
                    orderDetail.TotalPrice = (orderDetail.Quantity ?? 0) * (orderDetail.UnitPrice ?? 0);

                    _context.OrderDetails.Update(orderDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderDetailExists(orderDetail.OrderDetailId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Orders = _context.Orders
                                     .Select(o => new SelectListItem
                                     {
                                         Value = o.OrderId.ToString(),
                                         Text = o.InvoiceNo
                                     }).ToList();

            ViewBag.Products = _context.Products
                                       .Select(p => new SelectListItem
                                       {
                                           Value = p.ProductId.ToString(),
                                           Text = p.Name
                                       }).ToList();

            return View(orderDetail);
        }

        // GET: OrderDetail/Delete/5
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

        // POST: OrderDetail/Delete/5
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
            return _context.OrderDetails.Any(od => od.OrderDetailId == id);
        }
    }
}
