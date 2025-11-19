using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Saksupermarketsytemmvc.web.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Saksupermarketsytemmvc.web.Controllers
{
    [Authorize(Roles = "Admin,Cashier")]
    public class ReportsController : Controller
    {
        private readonly SaksoftSupermarketSystemContext _context;
        public ReportsController(SaksoftSupermarketSystemContext context) => _context = context;

        // Daily sales report
        public async Task<IActionResult> DailySales(DateTime? date)
        {
            var targetDate = date ?? DateTime.Today;
            var sales = await _context.Bills
                .Include(b => b.Customer)
                .Include(b => b.BillItems)
                    .ThenInclude(bi => bi.Product)
                .Where(b => b.BillDate.Date == targetDate.Date)
                .ToListAsync();

            ViewBag.Date = targetDate;
            return View(sales);
        }

     
        // Weekly sales report
        public async Task<IActionResult> WeeklySales(DateTime? startDate)
        {
            // If no start date selected, default to last 7 days including today
            var start = startDate ?? DateTime.Today.AddDays(-6);
            var end = start.AddDays(6); // 7-day period

            var sales = await _context.Bills
                .Include(b => b.Customer)
                .Include(b => b.BillItems)
                    .ThenInclude(bi => bi.Product)
                .Where(b => b.BillDate.Date >= start.Date && b.BillDate.Date <= end.Date)
                .ToListAsync();

            ViewBag.Start = start;
            ViewBag.End = end;
            ViewBag.SelectedDate = start; // keep selected date for view
            return View(sales);
        }

    }
}
