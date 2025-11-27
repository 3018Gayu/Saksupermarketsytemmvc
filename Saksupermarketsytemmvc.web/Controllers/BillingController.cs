using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Saksupermarketsytemmvc.web.Models;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Saksupermarketsytemmvc.web.Controllers
{
    [Authorize(Roles = "Cashier")]
    public class BillingController : Controller
    {
        private readonly SaksoftSupermarketSystemContext _context;
        public BillingController(SaksoftSupermarketSystemContext context) => _context = context;

        // GET: Billing/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Customers = new SelectList(await _context.Customers.ToListAsync(), "CustomerId", "CustomerName");
            ViewBag.Products = new SelectList(await _context.Products.ToListAsync(), "ProductId", "Name");
            return View();
        }

        // AJAX: Get product price
        [HttpGet]
        public async Task<IActionResult> GetProductPrice(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            return Json(product?.UnitPrice ?? 0);
        }

        // POST: Billing/CreateBill
        [HttpPost]
        public async Task<IActionResult> CreateBill([FromBody] Bill bill)
        {
            if (bill == null || bill.BillItems == null || !bill.BillItems.Any())
                return Json(new { success = false, message = "Bill cannot be empty." });

            bill.BillDate = DateTime.Now;

            if (bill.CustomerId == 0) bill.CustomerId = null;

            // Calculate total amount automatically
            bill.TotalAmount = bill.BillItems.Sum(i => i.Quantity * i.UnitPrice);

            _context.Bills.Add(bill);

            foreach (var item in bill.BillItems)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    product.StockQty -= item.Quantity;
                    _context.BillItems.Add(item);
                }
            }

            // ----------------- LOYALTY POINTS -----------------
            if (bill.CustomerId.HasValue)
            {
                var customer = await _context.Customers.FindAsync(bill.CustomerId.Value);
                if (customer != null)
                {
                    // 1 point per 10 rupees spent
                    int pointsEarned = (int)(bill.TotalAmount / 10);
                    customer.LoyaltyPoints += pointsEarned;
                    _context.Customers.Update(customer);
                }
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true, billId = bill.BillId });
        }

        // GET: Billing/Invoice/5
        public async Task<IActionResult> Invoice(int id)
        {
            var bill = await _context.Bills
                .Include(b => b.Customer)
                .Include(b => b.BillItems)
                    .ThenInclude(bi => bi.Product)
                .FirstOrDefaultAsync(b => b.BillId == id);

            if (bill == null) return NotFound();

            // Optional: Pass loyalty points earned in this bill to view
            ViewBag.PointsEarned = bill.CustomerId.HasValue ? (int)(bill.TotalAmount / 10) : 0;

            return View(bill);
        }
    }
}
