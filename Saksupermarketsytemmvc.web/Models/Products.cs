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

        [Required(ErrorMessage = "Product Name is required")]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Image URL is required")]
        [StringLength(500)]
        public string? ImageUrl { get; set; }

        [ForeignKey("Category")]
        public int? CategoryId { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Range(0.01, double.MaxValue)]
        public decimal? UnitPrice { get; set; }

        [Range(0, int.MaxValue)]
        public int? StockQty { get; set; }

        [Range(0, int.MaxValue)]
        public int MinimumStockLevel { get; set; } = 10;

        [DataType(DataType.Date)]
        public DateTime? ExpiryDate { get; set; }

        public bool? IsActive { get; set; } = true;

        public virtual Category? Category { get; set; }
        public virtual ICollection<BillItem> BillItems { get; set; } = new List<BillItem>();
        public virtual ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();
        public virtual ICollection<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();
    }
}
