using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Saksupermarketsytemmvc.web.Models
{
    public class Products
    {
        [Key]
        public int ProductId { get; set; }

        // Computed column in DB
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string? ProductCode { get; set; }

        [Required(ErrorMessage = "Product Name is required")]
        [StringLength(100, ErrorMessage = "Product Name cannot exceed 100 characters")]
        public string Name { get; set; } = null!;

        [ForeignKey("Category")]
        public int? CategoryId { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit Price must be greater than 0")]
        public decimal? UnitPrice { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock Quantity cannot be negative")]
        public int? StockQty { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ExpiryDate { get; set; }

        public bool? IsActive { get; set; } = true;


        // Navigation properties
        public virtual Category? Category { get; set; }

        public virtual ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();

        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
