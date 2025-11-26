using System.ComponentModel.DataAnnotations.Schema;

namespace Saksupermarketsytemmvc.web.Models
{
    public class OrderDetails
    {
        public int OrderDetailId { get; set; }

        public int? OrderId { get; set; }

        public int? ProductId { get; set; }

        public int? Quantity { get; set; }

        public decimal? UnitPrice { get; set; }

        // Computed by SQL Server, handles null values
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal TotalPrice { get; private set; } // make it readonly to prevent accidental updates

        public virtual Orders? Order { get; set; }
        public virtual Products? Product { get; set; }
    }
}
