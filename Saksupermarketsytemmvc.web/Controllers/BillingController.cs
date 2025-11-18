using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Saksupermarketsytemmvc.web.Models;

namespace Saksupermarketsytemmvc.web.Controllers
{
    public class BillingController : Controller
    {
        private readonly SaksoftSupermarketSystemContext _context;

        public BillingController(SaksoftSupermarketSystemContext context)
        {
            _context = context;
        }

        // GET: Billing/Create
        public async Task<IActionResult> Create()
        {
            // Fetch customers with correct property names
            var customers = await _context.Customers
                .Select(c => new { c.CustomerId, c.CustomerName })
                .ToListAsync();

            // Fetch products
            var products = await _context.Products
                .Select(p => new { p.ProductId, p.Name })
                .ToListAsync();

            ViewBag.Customers = new SelectList(customers, "CustomerId", "CustomerName");
            ViewBag.Products = new SelectList(products, "ProductId", "Name");

            return View();
        }

        // AJAX: Get product price by ID
        [HttpGet]
        public async Task<IActionResult> GetProductPrice(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return Json(0);
            return Json(product.UnitPrice);
        }

        // AJAX: Save Bill
        [HttpPost]
        public async Task<IActionResult> CreateBill([FromBody] Bill bill)
        {
            if (bill == null || bill.BillItems == null || !bill.BillItems.Any())
                return Json(new { success = false, message = "Bill cannot be empty." });

            // Save Bill Header
            var newBill = new Bill
            {
                CustomerId = bill.CustomerId,
                BillDate = DateTime.Now,
                TotalAmount = bill.TotalAmount
            };

            _context.Bills.Add(newBill);
            await _context.SaveChangesAsync();

            // Save Bill Items and reduce stock
            foreach (var item in bill.BillItems)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    product.StockQty -= item.Quantity;
                }

                var billItem = new BillItem
                {
                    BillId = newBill.BillId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                    // Use TotalPrice from model, do not set Total manually
                };

                _context.BillItems.Add(billItem);
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true, billId = newBill.BillId });
        }

        // GET: Billing/Invoice/{id}
        public async Task<IActionResult> Invoice(int id)
        {
            var bill = await _context.Bills
                .Include(b => b.Customer)
                .Include(b => b.BillItems)
                    .ThenInclude(bi => bi.Product)
                .FirstOrDefaultAsync(b => b.BillId == id);

            if (bill == null) return NotFound();

            return View(bill);
        }
    }
}
