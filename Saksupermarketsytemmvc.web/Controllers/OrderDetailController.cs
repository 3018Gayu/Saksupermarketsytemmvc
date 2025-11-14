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
    public class OrderDetailController : Controller
    {
        private readonly SaksoftSupermarketSystemContext _context;

        public OrderDetailController(SaksoftSupermarketSystemContext context)
        {
            _context = context;
        }

        // ========== INDEX ==========
        public async Task<IActionResult> Index(int orderId)
        {
            var orderDetails = await _context.OrderDetails
                                             .Include(od => od.Product)
                                             .Include(od => od.Order)
                                             .Where(od => od.OrderId == orderId)
                                             .ToListAsync();

            ViewBag.OrderId = orderId;

            if (TempData["SuccessMessage"] != null)
                ViewBag.SuccessMessage = TempData["SuccessMessage"];
            if (TempData["ErrorMessage"] != null)
                ViewBag.ErrorMessage = TempData["ErrorMessage"];

            return View(orderDetails);
        }

        // ========== DETAILS ==========
        public async Task<IActionResult> Details(int id)
        {
            var orderDetail = await _context.OrderDetails
                                            .Include(od => od.Product)
                                            .Include(od => od.Order)
                                            .FirstOrDefaultAsync(od => od.Orderdetailsid == id);

            if (orderDetail == null) return NotFound();

            return View(orderDetail);
        }

        // ========== CREATE ==========
        [HttpGet]
        public IActionResult Create(int orderId)
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "Name");
            ViewData["OrderId"] = orderId;
            return View(new OrderDetail { OrderId = orderId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderDetail orderDetail)
        {
            if (!ModelState.IsValid)
            {
                ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "Name", orderDetail.ProductId);
                ViewData["OrderId"] = orderDetail.OrderId;
                return View(orderDetail);
            }

            try
            {
                _context.OrderDetails.Add(orderDetail);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Order detail added successfully!";
                return RedirectToAction(nameof(Index), new { orderId = orderDetail.OrderId });
            }
            catch (DbUpdateException)
            {
                ViewBag.ErrorMessage = "Unable to add order detail. Please check related data.";
                ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "Name", orderDetail.ProductId);
                ViewData["OrderId"] = orderDetail.OrderId;
                return View(orderDetail);
            }
        }

        // ========== EDIT ==========
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var orderDetail = await _context.OrderDetails.FindAsync(id);
            if (orderDetail == null) return NotFound();

            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "Name", orderDetail.ProductId);
            return View(orderDetail);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OrderDetail orderDetail)
        {
            if (id != orderDetail.Orderdetailsid) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "Name", orderDetail.ProductId);
                return View(orderDetail);
            }

            try
            {
                _context.Update(orderDetail);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Order detail updated successfully!";
                return RedirectToAction(nameof(Index), new { orderId = orderDetail.OrderId });
            }
            catch (DbUpdateException)
            {
                ViewBag.ErrorMessage = "Unable to update order detail. Please try again later.";
                ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "Name", orderDetail.ProductId);
                return View(orderDetail);
            }
        }

        // ========== DELETE ==========
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var orderDetail = await _context.OrderDetails
                                            .Include(od => od.Product)
                                            .Include(od => od.Order)
                                            .FirstOrDefaultAsync(od => od.Orderdetailsid == id);

            if (orderDetail == null) return NotFound();

            if (TempData["ErrorMessage"] != null)
                ViewBag.ErrorMessage = TempData["ErrorMessage"];

            return View(orderDetail);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orderDetail = await _context.OrderDetails.FindAsync(id);
            if (orderDetail != null)
            {
                try
                {
                    _context.OrderDetails.Remove(orderDetail);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Order detail deleted successfully!";
                    return RedirectToAction(nameof(Index), new { orderId = orderDetail.OrderId });
                }
                catch (DbUpdateException)
                {
                    TempData["ErrorMessage"] = "Cannot delete this order detail because it’s referenced elsewhere.";
                    return RedirectToAction(nameof(Delete), new { id });
                }
            }

            TempData["ErrorMessage"] = "Order detail not found or already deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}
