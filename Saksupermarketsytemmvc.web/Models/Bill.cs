using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Saksupermarketsytemmvc.web.Models
{
    public class Bill
    {
        public int BillId { get; set; }
        public DateTime BillDate { get; set; } = DateTime.Now;
        public int? CustomerId { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal DiscountAmount { get; set; } = 0;
        public decimal DiscountPercent { get; set; } = 0;

        [NotMapped]
        public decimal NetAmount => TotalAmount - DiscountAmount - (TotalAmount * DiscountPercent / 100);

        public Customer? Customer { get; set; }
        public List<BillItem> BillItems { get; set; } = new();
    }
}
