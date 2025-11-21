using System;
using System.Collections.Generic;

namespace Saksupermarketsytemmvc.web.Models
{
    public class Dashboard
    {
        public int ProductsCount { get; set; }
        public int CategoryCount { get; set; }
        public int SupplierCount { get; set; }
        public int CustomerCount { get; set; }
        public int OrdersCount { get; set; }
        public int UserCount { get; set; }

        public List<Products> LowStockProducts { get; set; } = new List<Products>();
        public List<Products> ExpiredProducts { get; set; } = new List<Products>();
        public List<decimal> Last7DaysSales { get; set; } = new List<decimal>();
        public List<string> Last7DaysLabels { get; set; } = new List<string>();

    }
}
