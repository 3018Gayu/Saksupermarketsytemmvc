using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Saksupermarketsytemmvc.web.Models;
using System.Threading.Tasks;

namespace Saksupermarketsytemmvc.web.Controllers
{
    [Authorize(Roles = "Admin,Cashier")]
    public class OrderDetailController : Controller
    {
        private readonly SaksoftSupermarketSystemContext _context;
        public OrderDetailController(SaksoftSupermarketSystemContext context) => _context = context;

        public async Task<IActionResult> Index(int orderId)
        {
            var orderDetails = await _context.OrderDetails
                                             .Include(od => od.Product)
                                             .Include(od => od.Order)
                                             .Where(od => od.OrderId == orderId)
                                             .ToListAsync();
            return View(orderDetails);
        }

        [HttpGet]
        public IActionResult Create(int orderId)
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "Name");
            ViewData["OrderId"] = orderId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderDetail orderDetail)
        {
            if (ModelState.IsValid)
            {
                _context.OrderDetails.Add(orderDetail);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Order detail added successfully!";
                return RedirectToAction(nameof(Index), new { orderId = orderDetail.OrderId });
            }
            return View(orderDetail);
        }
    }
}
