using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Saksupermarketsytemmvc.web.Models
{
    [Table("orders")]
    public class Orders
    {
        public int OrderId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string? InvoiceNo { get; set; }

        public int? CustomerId { get; set; }

        public DateTime? OrderDate { get; set; }

        public decimal? TotalAmount { get; set; }

        public decimal? TaxAmount { get; set; }

        public decimal? Discount { get; set; }

        public decimal? NetAmount { get; set; }

        // Navigation property
        public virtual Customer? Customer { get; set; }

        public virtual ICollection<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();
    }
}
