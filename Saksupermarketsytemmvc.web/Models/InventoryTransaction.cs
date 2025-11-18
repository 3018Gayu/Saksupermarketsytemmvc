using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Saksupermarketsytemmvc.web.Models
{
    public partial class InventoryTransaction
    {
        [Key]
        public int TransId { get; set; }

        [ForeignKey("Product")]
        public int? ProductId { get; set; }

        public int? Quantity { get; set; }

        [StringLength(50)]
        public string? Type { get; set; }

        [StringLength(255)]
        public string? Remarks { get; set; }

        public DateTime? Date { get; set; }

        public virtual Products? Product { get; set; }
    }
}
