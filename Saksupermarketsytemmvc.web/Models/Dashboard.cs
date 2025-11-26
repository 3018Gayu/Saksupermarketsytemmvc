using System;
using System.Collections.Generic;

namespace Saksupermarketsytemmvc.web.Models
{
    public class Dashboard
    {
        // Admin
        public decimal TotalSales { get; set; } = 0;
        public int CustomerCount { get; set; } = 0;
        public int LowStockCount { get; set; } = 0;
        public int UserCount { get; set; } = 0;
        public List<RecentTransactionDTO> RecentTransactions { get; set; } = new List<RecentTransactionDTO>();
        public List<string> Last7DaysLabels { get; set; } = new List<string>();
        public List<decimal> Last7DaysSales { get; set; } = new List<decimal>();
        public int OrdersCount { get; set; }
        public int OrderDetailsCount { get; set; }

        // Cashier
        public decimal TodaysSales { get; set; } = 0;
        public int TodaysInvoices { get; set; } = 0;
        public List<RecentTransactionDTO> RecentBills { get; set; } = new List<RecentTransactionDTO>();

        // Inventory Manager
        public int ProductsCount { get; set; } = 0;
        public int SupplierCount { get; set; } = 0;
        public List<Products> LowStockProducts { get; set; } = new List<Products>();
        public List<Products> ExpiredProducts { get; set; } = new List<Products>();
        public List<Products> RecentProducts { get; set; } = new List<Products>();
    }

    public class RecentTransactionDTO
    {
        public string Date { get; set; } = string.Empty;
        public string InvoiceNo { get; set; } = string.Empty;
        public string Customer { get; set; } = "Walk-in";
        public decimal Amount { get; set; } = 0;
        public string Cashier { get; set; } = "N/A";
    }
}
