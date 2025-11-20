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
using OfficeOpenXml;
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
        public ReportsController(SaksoftSupermarketSystemContext context) => _context = context;

        // ---------------- REPORTS DASHBOARD --------------------
        public IActionResult Index() => View();

        // ---------------- INVENTORY REPORT --------------------
        public async Task<IActionResult> InventoryReport(int? categoryId)
        {
            var query = _context.InventoryTransactions
                .Include(t => t.Product)
                .ThenInclude(p => p.Category)
                .AsQueryable();

            if (categoryId.HasValue && categoryId.Value > 0)
                query = query.Where(t => t.Product.CategoryId == categoryId.Value);

            var transactions = await query.ToListAsync();
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.CategoryId = categoryId ?? 0;

            return View(transactions);
        }

        public async Task<IActionResult> InventoryReportPdf(int? categoryId)
        {
            var query = _context.InventoryTransactions
                .Include(t => t.Product)
                .ThenInclude(p => p.Category)
                .AsQueryable();

            if (categoryId.HasValue && categoryId.Value > 0)
                query = query.Where(t => t.Product.CategoryId == categoryId.Value);

            var transactions = await query.ToListAsync();

            using var stream = new MemoryStream();
            var writer = new PdfWriter(stream);
            var pdf = new PdfDocument(writer);
            var doc = new Document(pdf);
            PdfFont bold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

            doc.Add(new Paragraph("Inventory Transactions Report")
                .SetFont(bold).SetFontSize(16));

            var table = new Table(UnitValue.CreatePercentArray(new float[] { 2, 4, 2, 2, 3, 4 }))
                .UseAllAvailableWidth();

            table.AddHeaderCell("Transaction ID");
            table.AddHeaderCell("Product");
            table.AddHeaderCell("Quantity");
            table.AddHeaderCell("Type");
            table.AddHeaderCell("Date");
            table.AddHeaderCell("Remarks");

            foreach (var t in transactions)
            {
                table.AddCell(t.TransId.ToString());
                table.AddCell(t.Product?.Name ?? "N/A");
                table.AddCell(t.Quantity.ToString());
                table.AddCell(t.Type);
                table.AddCell(t.Date?.ToString("yyyy-MM-dd HH:mm") ?? "N/A");
                table.AddCell(t.Remarks ?? "");
            }

            doc.Add(table);
            doc.Close();
            return File(stream.ToArray(), "application/pdf", "InventoryReport.pdf");
        }

        public async Task<IActionResult> InventoryReportExcel(int? categoryId)
        {
            var query = _context.InventoryTransactions
                .Include(t => t.Product)
                .ThenInclude(p => p.Category)
                .AsQueryable();

            if (categoryId.HasValue && categoryId.Value > 0)
                query = query.Where(t => t.Product.CategoryId == categoryId.Value);

            var transactions = await query.ToListAsync();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Inventory Transactions");
            sheet.Cells["A1"].Value = "Inventory Transactions Report";

            sheet.Cells["A3"].Value = "Transaction ID";
            sheet.Cells["B3"].Value = "Product";
            sheet.Cells["C3"].Value = "Quantity";
            sheet.Cells["D3"].Value = "Type";
            sheet.Cells["E3"].Value = "Date";
            sheet.Cells["F3"].Value = "Remarks";

            int row = 4;
            foreach (var t in transactions)
            {
                sheet.Cells[row, 1].Value = t.TransId;
                sheet.Cells[row, 2].Value = t.Product?.Name ?? "N/A";
                sheet.Cells[row, 3].Value = t.Quantity;
                sheet.Cells[row, 4].Value = t.Type;
                sheet.Cells[row, 5].Value = t.Date?.ToString("yyyy-MM-dd HH:mm");
                sheet.Cells[row, 6].Value = t.Remarks ?? "";
                row++;
            }

            return File(package.GetAsByteArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "InventoryReport.xlsx");
        }

        // ---------------- SALES REPORT --------------------
        public async Task<IActionResult> SalesReport(DateTime? startDate, DateTime? endDate)
        {
            var start = startDate ?? DateTime.Today.AddDays(-6);
            var end = endDate ?? DateTime.Today;

            var sales = await _context.Bills
                .Include(b => b.Customer)
                .Include(b => b.BillItems).ThenInclude(bi => bi.Product)
                .Where(b => b.BillDate.Date >= start && b.BillDate.Date <= end)
                .ToListAsync();

            ViewBag.Start = start;
            ViewBag.End = end;
            return View(sales);
        }

        public async Task<IActionResult> SalesReportPdf(DateTime? startDate, DateTime? endDate)
        {
            var start = startDate ?? DateTime.Today.AddDays(-6);
            var end = endDate ?? DateTime.Today;

            var sales = await _context.Bills
                .Include(b => b.Customer)
                .Include(b => b.BillItems).ThenInclude(bi => bi.Product)
                .Where(b => b.BillDate.Date >= start && b.BillDate.Date <= end)
                .ToListAsync();

            using var stream = new MemoryStream();
            var writer = new PdfWriter(stream);
            var pdf = new PdfDocument(writer);
            var doc = new Document(pdf);
            PdfFont bold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

            doc.Add(new Paragraph($"Sales Report ({start:dd MMM yyyy} - {end:dd MMM yyyy})")
                .SetFont(bold).SetFontSize(16));

            var table = new Table(UnitValue.CreatePercentArray(new float[] { 2, 4, 3, 2 })).UseAllAvailableWidth();
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
            return File(stream.ToArray(), "application/pdf", "SalesReport.pdf");
        }

        public async Task<IActionResult> SalesReportExcel(DateTime? startDate, DateTime? endDate)
        {
            var start = startDate ?? DateTime.Today.AddDays(-6);
            var end = endDate ?? DateTime.Today;

            var sales = await _context.Bills
                .Include(b => b.Customer)
                .Include(b => b.BillItems).ThenInclude(bi => bi.Product)
                .Where(b => b.BillDate.Date >= start && b.BillDate.Date <= end)
                .ToListAsync();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Sales Report");
            sheet.Cells["A1"].Value = $"Sales Report ({start:dd MMM yyyy} - {end:dd MMM yyyy})";

            sheet.Cells["A3"].Value = "Invoice";
            sheet.Cells["B3"].Value = "Customer";
            sheet.Cells["C3"].Value = "Date";
            sheet.Cells["D3"].Value = "Amount";

            int row = 4;
            foreach (var bill in sales)
            {
                sheet.Cells[row, 1].Value = bill.BillId;
                sheet.Cells[row, 2].Value = bill.Customer?.CustomerName ?? "Walk-in";
                sheet.Cells[row, 3].Value = bill.BillDate.ToString("dd/MM/yyyy");
                sheet.Cells[row, 4].Value = bill.TotalAmount;
                row++;
            }

            return File(package.GetAsByteArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "SalesReport.xlsx");
        }
        // ---------------- CUSTOMER REPORT --------------------
        public async Task<IActionResult> CustomerReport(DateTime? startDate, DateTime? endDate)
        {
            var start = startDate ?? DateTime.Today.AddDays(-30);
            var end = endDate ?? DateTime.Today;

            var customers = await _context.Bills
                .Include(b => b.Customer)
                .Where(b => b.BillDate.Date >= start && b.BillDate.Date <= end)
                .Select(b => b.Customer)
                .Where(c => c != null)
                .Distinct()
                .ToListAsync();

            ViewBag.Start = start;
            ViewBag.End = end;

            return View(customers);
        }

        public async Task<IActionResult> CustomerReportPdf(DateTime? startDate, DateTime? endDate)
        {
            var start = startDate ?? DateTime.Today.AddDays(-30);
            var end = endDate ?? DateTime.Today;

            var customers = await _context.Bills
                .Include(b => b.Customer)
                .Where(b => b.BillDate.Date >= start && b.BillDate.Date <= end)
                .Select(b => b.Customer)
                .Where(c => c != null)
                .Distinct()
                .ToListAsync();

            using var stream = new MemoryStream();
            var writer = new PdfWriter(stream);
            var pdf = new PdfDocument(writer);
            var doc = new Document(pdf);
            var bold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

            doc.Add(new Paragraph($"Customer Report ({start:dd MMM yyyy} - {end:dd MMM yyyy})")
                .SetFont(bold).SetFontSize(16));

            var table = new Table(UnitValue.CreatePercentArray(new float[] { 2, 4, 4, 3, 5 }))
                .UseAllAvailableWidth();
            table.AddHeaderCell("Customer ID");
            table.AddHeaderCell("Name");
            table.AddHeaderCell("Email");
            table.AddHeaderCell("Phone");
            table.AddHeaderCell("Address");

            foreach (var c in customers)
            {
                table.AddCell(c.CustomerId.ToString());
                table.AddCell(c.CustomerName);
                table.AddCell(c.CustEmail ?? "");
                table.AddCell(c.CustPhone ?? "");
                table.AddCell(c.CustAddress ?? "");
            }

            doc.Add(table);
            doc.Close();

            return File(stream.ToArray(), "application/pdf", "CustomerReport.pdf");
        }

        public async Task<IActionResult> CustomerReportExcel(DateTime? startDate, DateTime? endDate)
        {
            var start = startDate ?? DateTime.Today.AddDays(-30);
            var end = endDate ?? DateTime.Today;

            var customers = await _context.Bills
                .Include(b => b.Customer)
                .Where(b => b.BillDate.Date >= start && b.BillDate.Date <= end)
                .Select(b => b.Customer)
                .Where(c => c != null)
                .Distinct()
                .ToListAsync();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Customer Report");

            sheet.Cells["A1"].Value = $"Customer Report ({start:dd MMM yyyy} - {end:dd MMM yyyy})";
            sheet.Cells["A3"].Value = "Customer ID";
            sheet.Cells["B3"].Value = "Name";
            sheet.Cells["C3"].Value = "Email";
            sheet.Cells["D3"].Value = "Phone";
            sheet.Cells["E3"].Value = "Address";

            int row = 4;
            foreach (var c in customers)
            {
                sheet.Cells[row, 1].Value = c.CustomerId;
                sheet.Cells[row, 2].Value = c.CustomerName;
                sheet.Cells[row, 3].Value = c.CustEmail ?? "";
                sheet.Cells[row, 4].Value = c.CustPhone ?? "";
                sheet.Cells[row, 5].Value = c.CustAddress ?? "";
                row++;
            }

            return File(package.GetAsByteArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "CustomerReport.xlsx");
        }


        // ---------------- DAILY SALES --------------------
        public async Task<IActionResult> DailySales(DateTime? date, int? categoryId)
        {
            var selectedDate = date ?? DateTime.Today;

            var salesQuery = _context.Bills
                .Include(b => b.Customer)
                .Include(b => b.BillItems).ThenInclude(bi => bi.Product)
                .Where(b => b.BillDate.Date == selectedDate.Date);

            if (categoryId.HasValue && categoryId.Value > 0)
                salesQuery = salesQuery.Where(b => b.BillItems.Any(bi => bi.Product.CategoryId == categoryId.Value));

            var sales = await salesQuery.ToListAsync();
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Date = selectedDate;
            ViewBag.CategoryId = categoryId ?? 0;

            return View(sales);
        }

        // ---------------- WEEKLY SALES --------------------
        public async Task<IActionResult> WeeklySales(DateTime? startDate, int? categoryId)
        {
            var start = startDate ?? DateTime.Today.AddDays(-6);
            var end = start.AddDays(6);

            var salesQuery = _context.Bills
                .Include(b => b.Customer)
                .Include(b => b.BillItems).ThenInclude(bi => bi.Product)
                .Where(b => b.BillDate.Date >= start && b.BillDate.Date <= end);

            if (categoryId.HasValue && categoryId.Value > 0)
                salesQuery = salesQuery.Where(b => b.BillItems.Any(bi => bi.Product.CategoryId == categoryId.Value));

            var sales = await salesQuery.ToListAsync();
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Start = start;
            ViewBag.End = end;
            ViewBag.CategoryId = categoryId ?? 0;

            return View(sales);
        }

        // ---------------- MONTHLY SALES --------------------
        public async Task<IActionResult> MonthlySales(int? month, int? year, int? categoryId)
        {
            int selectedYear = year ?? DateTime.Today.Year;
            int selectedMonth = month ?? DateTime.Today.Month;

            var salesQuery = _context.Bills
                .Include(b => b.Customer)
                .Include(b => b.BillItems).ThenInclude(bi => bi.Product)
                .Where(b => b.BillDate.Month == selectedMonth && b.BillDate.Year == selectedYear);

            if (categoryId.HasValue && categoryId.Value > 0)
                salesQuery = salesQuery.Where(b => b.BillItems.Any(bi => bi.Product.CategoryId == categoryId.Value));

            var sales = await salesQuery.ToListAsync();
            ViewBag.Month = selectedMonth;
            ViewBag.Year = selectedYear;
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.CategoryId = categoryId ?? 0;

            return View(sales);
        }

        public async Task<IActionResult> MonthlySalesPdf(int? month, int? year, int? categoryId)
        {
            int selectedYear = year ?? DateTime.Today.Year;
            int selectedMonth = month ?? DateTime.Today.Month;

            var salesQuery = _context.Bills
                .Include(b => b.Customer)
                .Include(b => b.BillItems).ThenInclude(bi => bi.Product)
                .Where(b => b.BillDate.Month == selectedMonth && b.BillDate.Year == selectedYear);

            if (categoryId.HasValue && categoryId.Value > 0)
                salesQuery = salesQuery.Where(b => b.BillItems.Any(bi => bi.Product.CategoryId == categoryId.Value));

            var sales = await salesQuery.ToListAsync();

            using var stream = new MemoryStream();
            var writer = new PdfWriter(stream);
            var pdf = new PdfDocument(writer);
            var doc = new Document(pdf);
            PdfFont bold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

            doc.Add(new Paragraph($"Monthly Sales Report - {selectedMonth:00}/{selectedYear}")
                .SetFont(bold).SetFontSize(16));

            var table = new Table(UnitValue.CreatePercentArray(new float[] { 2, 4, 3, 2 })).UseAllAvailableWidth();
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
            return File(stream.ToArray(), "application/pdf", "MonthlySalesReport.pdf");
        }

        public async Task<IActionResult> MonthlySalesExcel(int? month, int? year, int? categoryId)
        {
            int selectedYear = year ?? DateTime.Today.Year;
            int selectedMonth = month ?? DateTime.Today.Month;

            var salesQuery = _context.Bills
                .Include(b => b.Customer)
                .Include(b => b.BillItems).ThenInclude(bi => bi.Product)
                .Where(b => b.BillDate.Month == selectedMonth && b.BillDate.Year == selectedYear);

            if (categoryId.HasValue && categoryId.Value > 0)
                salesQuery = salesQuery.Where(b => b.BillItems.Any(bi => bi.Product.CategoryId == categoryId.Value));

            var sales = await salesQuery.ToListAsync();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Monthly Sales");
            sheet.Cells["A1"].Value = $"Monthly Sales Report - {selectedMonth:00}/{selectedYear}";
            sheet.Cells["A3"].Value = "Invoice";
            sheet.Cells["B3"].Value = "Customer";
            sheet.Cells["C3"].Value = "Date";
            sheet.Cells["D3"].Value = "Amount";

            int row = 4;
            foreach (var bill in sales)
            {
                sheet.Cells[row, 1].Value = bill.BillId;
                sheet.Cells[row, 2].Value = bill.Customer?.CustomerName ?? "Walk-in";
                sheet.Cells[row, 3].Value = bill.BillDate.ToString("dd/MM/yyyy");
                sheet.Cells[row, 4].Value = bill.TotalAmount;
                row++;
            }

            return File(package.GetAsByteArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "MonthlySalesReport.xlsx");
        }
    }
}
