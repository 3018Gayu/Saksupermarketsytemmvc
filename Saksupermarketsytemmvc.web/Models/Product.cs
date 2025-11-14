using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Saksupermarketsytemmvc.web.Models
{
    public partial class Product
    {
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Product Name is required")]
        [StringLength(100, ErrorMessage = "Product Name cannot exceed 100 characters")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Category is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid category")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Unit Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit Price must be greater than 0")]
        [DataType(DataType.Currency)]
        public decimal UnitPrice { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock Quantity cannot be negative")]
        public int? StockQty { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ExpiryDate { get; set; } // Optional expiry

        // Navigation properties
        public virtual Category Category { get; set; } = null!;
        public virtual ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
