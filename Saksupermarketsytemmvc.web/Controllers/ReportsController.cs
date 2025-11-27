using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Saksupermarketsytemmvc.web.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Saksupermarketsytemmvc.web.Controllers
{
    [Authorize(Roles = "Admin,Cashier")]
    public class ReportsController : Controller
    {
        private readonly SaksoftSupermarketSystemContext _context;

        private const string LogoUrl =
            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTW-3A1M8Ni_1sPNfH9zM3UH7BrM_wM9rShfw&s";

        public ReportsController(SaksoftSupermarketSystemContext context)
        {
            _context = context;
        }

        // ======================================================================
        // UNIVERSAL HEADER
        // ======================================================================

        private void AddHeader(Document doc, string title)
        {
            PdfFont bold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

            try
            {
                Image logo = new Image(ImageDataFactory.Create(LogoUrl))
                    .SetWidth(90)
                    .SetHorizontalAlignment(HorizontalAlignment.CENTER);

                doc.Add(logo);
            }
            catch { }

            doc.Add(new Paragraph("SAKSOFT SUPERMARKET")
                .SetFont(bold)
                .SetFontSize(18)
                .SetTextAlignment(TextAlignment.CENTER));

            doc.Add(new Paragraph(title)
                .SetFont(bold)
                .SetFontSize(15)
                .SetTextAlignment(TextAlignment.CENTER));

            doc.Add(new Paragraph("\n"));
        }

        public IActionResult Index() => View();

        // ======================================================================
        // CUSTOMER REPORT (FIXED — NO CreatedDate NEEDED)
        // ======================================================================

        public async Task<IActionResult> CustomerReport()
        {
            var customers = await _context.Customers.ToListAsync();
            ViewBag.Start = DateTime.Today.AddDays(-30);
            ViewBag.End = DateTime.Today;
            return View(customers);
        }

        public async Task<IActionResult> CustomerReportPdf()
        {
            var customers = await _context.Customers.ToListAsync();

            using MemoryStream stream = new();
            PdfWriter writer = new(stream);
            PdfDocument pdf = new(writer);
            Document doc = new(pdf);

            AddHeader(doc, "Customer Report");

            var table = new Table(UnitValue.CreatePercentArray(
                new float[] { 2, 4, 4, 3, 4, 2 }))
                .UseAllAvailableWidth();

            table.AddHeaderCell("Customer ID");
            table.AddHeaderCell("Name");
            table.AddHeaderCell("Email");
            table.AddHeaderCell("Phone");
            table.AddHeaderCell("Address");
            table.AddHeaderCell("Points");

            foreach (var c in customers)
            {
                table.AddCell(c.CustomerId.ToString());
                table.AddCell(c.CustomerName ?? "-");
                table.AddCell(c.CustEmail ?? "-");
                table.AddCell(c.CustPhone ?? "-");
                table.AddCell(c.CustAddress ?? "-");
                table.AddCell(c.LoyaltyPoints.ToString());
            }

            doc.Add(table);
            doc.Close();

            return File(stream.ToArray(), "application/pdf", "CustomerReport.pdf");
        }

        // ======================================================================
        // DAILY SALES REPORT
        // ======================================================================

        public async Task<IActionResult> DailySales(DateTime? date, int? categoryId)
        {
            DateTime selectedDate = date ?? DateTime.Today;

            var query = _context.Bills
                .Include(b => b.Customer)
                .Include(b => b.BillItems).ThenInclude(bi => bi.Product)
                .Where(b => b.BillDate.Date == selectedDate.Date);

            if (categoryId.HasValue && categoryId > 0)
                query = query.Where(b =>
                    b.BillItems.Any(bi => bi.Product.CategoryId == categoryId));

            var sales = await query.ToListAsync();

            ViewBag.Date = selectedDate;
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.CategoryId = categoryId ?? 0;

            return View(sales);
        }

        public async Task<IActionResult> DailySalesPdf(DateTime? date, int? categoryId)
        {
            DateTime selectedDate = date ?? DateTime.Today;

            var query = _context.Bills
                .Include(b => b.Customer)
                .Include(b => b.BillItems).ThenInclude(bi => bi.Product)
                .Where(b => b.BillDate.Date == selectedDate.Date);

            if (categoryId.HasValue && categoryId > 0)
                query = query.Where(b =>
                    b.BillItems.Any(bi => bi.Product.CategoryId == categoryId));

            var sales = await query.ToListAsync();

            using MemoryStream stream = new();
            PdfWriter writer = new(stream);
            PdfDocument pdf = new(writer);
            Document doc = new(pdf);

            AddHeader(doc, $"Daily Sales Report - {selectedDate:dd MMM yyyy}");

            var table = new Table(UnitValue.CreatePercentArray(
                new float[] { 2, 4, 3, 2 }))
                .UseAllAvailableWidth();

            table.AddHeaderCell("Bill ID");
            table.AddHeaderCell("Customer");
            table.AddHeaderCell("Date");
            table.AddHeaderCell("Amount");

            foreach (var bill in sales)
            {
                table.AddCell(bill.BillId.ToString());
                table.AddCell(bill.Customer?.CustomerName ?? "Walk-in");
                table.AddCell(bill.BillDate.ToString("dd/MM/yyyy"));
                table.AddCell(bill.TotalAmount.ToString("C"));
            }

            doc.Add(table);
            doc.Close();

            return File(stream.ToArray(), "application/pdf", "DailySalesReport.pdf");
        }

        // ======================================================================
        // WEEKLY SALES
        // ======================================================================

        public async Task<IActionResult> WeeklySales(DateTime? startDate, int? categoryId)
        {
            DateTime start = startDate ?? DateTime.Today.AddDays(-6);
            DateTime end = start.AddDays(6);

            var query = _context.Bills
                .Include(b => b.Customer)
                .Include(b => b.BillItems).ThenInclude(bi => bi.Product)
                .Where(b => b.BillDate >= start && b.BillDate <= end);

            if (categoryId.HasValue && categoryId > 0)
                query = query.Where(b =>
                    b.BillItems.Any(bi => bi.Product.CategoryId == categoryId));

            var sales = await query.ToListAsync();

            ViewBag.Start = start;
            ViewBag.End = end;
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.CategoryId = categoryId ?? 0;

            return View(sales);
        }

        public async Task<IActionResult> WeeklySalesPdf(DateTime? startDate, int? categoryId)
        {
            DateTime start = startDate ?? DateTime.Today.AddDays(-6);
            DateTime end = start.AddDays(6);

            var query = _context.Bills
                .Include(b => b.Customer)
                .Include(b => b.BillItems).ThenInclude(bi => bi.Product)
                .Where(b => b.BillDate >= start && b.BillDate <= end);

            if (categoryId.HasValue && categoryId > 0)
                query = query.Where(b =>
                    b.BillItems.Any(bi => bi.Product.CategoryId == categoryId));

            var sales = await query.ToListAsync();

            using MemoryStream stream = new();
            PdfWriter writer = new(stream);
            PdfDocument pdf = new(writer);
            Document doc = new(pdf);

            AddHeader(doc,
                $"Weekly Sales Report - {start:dd MMM yyyy} to {end:dd MMM yyyy}");

            var table = new Table(UnitValue.CreatePercentArray(
                new float[] { 2, 4, 3, 2 }))
                .UseAllAvailableWidth();

            table.AddHeaderCell("Bill ID");
            table.AddHeaderCell("Customer");
            table.AddHeaderCell("Date");
            table.AddHeaderCell("Amount");

            foreach (var bill in sales)
            {
                table.AddCell(bill.BillId.ToString());
                table.AddCell(bill.Customer?.CustomerName ?? "Walk-in");
                table.AddCell(bill.BillDate.ToString("dd/MM/yyyy"));
                table.AddCell(bill.TotalAmount.ToString("C"));
            }

            doc.Add(table);
            doc.Close();

            return File(stream.ToArray(),
                "application/pdf",
                "WeeklySalesReport.pdf");
        }

        // ======================================================================
        // INVENTORY REPORT
        // ======================================================================

        public async Task<IActionResult> InventoryReport(int? categoryId)
        {
            var query = _context.InventoryTransactions
                .Include(t => t.Product).ThenInclude(p => p.Category)
                .AsQueryable();

            if (categoryId.HasValue && categoryId > 0)
                query = query.Where(t => t.Product.CategoryId == categoryId);

            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.CategoryId = categoryId ?? 0;

            return View(await query.ToListAsync());
        }

        public async Task<IActionResult> InventoryReportPdf(int? categoryId)
        {
            var query = _context.InventoryTransactions
                .Include(t => t.Product).ThenInclude(p => p.Category)
                .AsQueryable();

            if (categoryId.HasValue && categoryId > 0)
                query = query.Where(t => t.Product.CategoryId == categoryId);

            var transactions = await query.ToListAsync();

            using MemoryStream stream = new();
            PdfWriter writer = new(stream);
            PdfDocument pdf = new(writer);
            Document doc = new(pdf);

            AddHeader(doc, "Inventory Transactions Report");

            var table = new Table(UnitValue.CreatePercentArray(
                new float[] { 2, 4, 2, 2, 3, 4 }))
                .UseAllAvailableWidth();

            table.AddHeaderCell("Trans ID");
            table.AddHeaderCell("Product");
            table.AddHeaderCell("Qty");
            table.AddHeaderCell("Type");
            table.AddHeaderCell("Date");
            table.AddHeaderCell("Remarks");

            foreach (var t in transactions)
            {
                table.AddCell(t.TransId.ToString());
                table.AddCell(t.Product?.Name ?? "N/A");
                table.AddCell(t.Quantity.ToString());
                table.AddCell(t.Type ?? "N/A");
                table.AddCell(t.Date?.ToString("dd/MM/yyyy HH:mm") ?? "N/A");
                table.AddCell(t.Remarks ?? "-");
            }

            doc.Add(table);
            doc.Close();

            return File(stream.ToArray(),
                "application/pdf",
                "InventoryReport.pdf");
        }

        // ======================================================================
        // SALES REPORT (RANGE)
        // ======================================================================

        public async Task<IActionResult> SalesReport(DateTime? startDate, DateTime? endDate)
        {
            DateTime start = startDate ?? DateTime.Today.AddDays(-6);
            DateTime end = endDate ?? DateTime.Today;

            var sales = await _context.Bills
                .Include(b => b.Customer)
                .Include(b => b.BillItems).ThenInclude(b => b.Product)
                .Where(b => b.BillDate >= start && b.BillDate <= end)
                .ToListAsync();

            ViewBag.Start = start;
            ViewBag.End = end;

            return View(sales);
        }

        public async Task<IActionResult> SalesReportPdf(DateTime? startDate, DateTime? endDate)
        {
            DateTime start = startDate ?? DateTime.Today.AddDays(-6);
            DateTime end = endDate ?? DateTime.Today;

            var sales = await _context.Bills
                .Include(b => b.Customer)
                .Include(b => b.BillItems).ThenInclude(b => b.Product)
                .Where(b => b.BillDate >= start && b.BillDate <= end)
                .ToListAsync();

            using MemoryStream stream = new();
            PdfWriter writer = new(stream);
            PdfDocument pdf = new(writer);
            Document doc = new(pdf);

            AddHeader(doc,
                $"Sales Report {start:dd MMM yyyy} - {end:dd MMM yyyy}");

            var table = new Table(UnitValue.CreatePercentArray(
                new float[] { 2, 4, 3, 2 }))
                .UseAllAvailableWidth();

            table.AddHeaderCell("Invoice No");
            table.AddHeaderCell("Customer");
            table.AddHeaderCell("Date");
            table.AddHeaderCell("Amount");

            foreach (var bill in sales)
            {
                table.AddCell(bill.BillId.ToString());
                table.AddCell(bill.Customer?.CustomerName ?? "Walk-in");
                table.AddCell(bill.BillDate.ToString("dd/MM/yyyy"));
                table.AddCell(bill.TotalAmount.ToString("C"));
            }

            doc.Add(table);
            doc.Close();

            return File(stream.ToArray(),
                "application/pdf",
                "SalesReport.pdf");
        }

        // ======================================================================
        // MONTHLY SALES REPORT
        // ======================================================================

        public async Task<IActionResult> MonthlySales(int? month, int? year, int? categoryId)
        {
            int m = month ?? DateTime.Today.Month;
            int y = year ?? DateTime.Today.Year;

            var query = _context.Bills
                .Include(b => b.Customer)
                .Include(b => b.BillItems).ThenInclude(b => b.Product)
                .Where(b => b.BillDate.Month == m && b.BillDate.Year == y);

            if (categoryId.HasValue && categoryId > 0)
                query = query.Where(b =>
                    b.BillItems.Any(bi => bi.Product.CategoryId == categoryId));

            ViewBag.Month = m;
            ViewBag.Year = y;
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.CategoryId = categoryId ?? 0;

            return View(await query.ToListAsync());
        }

        public async Task<IActionResult> MonthlySalesPdf(int? month, int? year, int? categoryId)
        {
            int m = month ?? DateTime.Today.Month;
            int y = year ?? DateTime.Today.Year;

            var query = _context.Bills
                .Include(b => b.Customer)
                .Include(b => b.BillItems).ThenInclude(b => b.Product)
                .Where(b => b.BillDate.Month == m && b.BillDate.Year == y);

            if (categoryId.HasValue && categoryId > 0)
                query = query.Where(b =>
                    b.BillItems.Any(bi => bi.Product.CategoryId == categoryId));

            var sales = await query.ToListAsync();

            using MemoryStream stream = new();
            PdfWriter writer = new(stream);
            PdfDocument pdf = new(writer);
            Document doc = new(pdf);

            AddHeader(doc, $"Monthly Sales Report - {m:00}/{y}");

            var table = new Table(UnitValue.CreatePercentArray(
                new float[] { 2, 4, 3, 2 }))
                .UseAllAvailableWidth();

            table.AddHeaderCell("Invoice No");
            table.AddHeaderCell("Customer");
            table.AddHeaderCell("Date");
            table.AddHeaderCell("Amount");

            foreach (var bill in sales)
            {
                table.AddCell(bill.BillId.ToString());
                table.AddCell(bill.Customer?.CustomerName ?? "Walk-in");
                table.AddCell(bill.BillDate.ToString("dd/MM/yyyy"));
                table.AddCell(bill.TotalAmount.ToString("C"));
            }

            doc.Add(table);
            doc.Close();

            return File(stream.ToArray(),
                "application/pdf",
                "MonthlySalesReport.pdf");
        }
    }
}
