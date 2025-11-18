using System;
using System.Collections.Generic;

namespace Saksupermarketsytemmvc.web.Models
{
    public class Bill
    {
        public int BillId { get; set; }

        public DateTime BillDate { get; set; } = DateTime.Now;

        public int? CustomerId { get; set; }

        public decimal TotalAmount { get; set; }

        public Customer? Customer { get; set; }

        public List<BillItem> BillItems { get; set; } = new();
    }
}
