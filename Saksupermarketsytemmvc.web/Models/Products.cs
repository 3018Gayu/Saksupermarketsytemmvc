using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Saksupermarketsytemmvc.web.Models
{
    public class Products
    {
        [Key]
        public int ProductId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string? ProductCode { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = null!;

        [Required, StringLength(500)]
        public string? ImageUrl { get; set; }

        public int? CategoryId { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? UnitPrice { get; set; }

        public int? StockQty { get; set; }
        public int MinimumStockLevel { get; set; } = 10;

        public DateTime? ExpiryDate { get; set; }
        public bool? IsActive { get; set; } = true;

        [Range(0, 100)]
        [Column(TypeName = "decimal(5,2)")]
        public decimal ProductDiscountRate { get; set; } = 0;

        [Range(0, 100)]
        [Column(TypeName = "decimal(5,2)")]
        public decimal ProductTaxRate { get; set; } = 0;

        public virtual ICollection<BillItem> BillItems { get; set; } = new List<BillItem>();
        public virtual ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();
        public virtual ICollection<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();
        public virtual Category? Category { get; set; }

        [NotMapped]
        public string StockStatus
        {
            get
            {
                int qty = StockQty ?? 0;
                if (qty == 0) return "Out of Stock";
                if (qty < MinimumStockLevel) return "Low Stock";
                return "In Stock";
            }
        }
    }
}
