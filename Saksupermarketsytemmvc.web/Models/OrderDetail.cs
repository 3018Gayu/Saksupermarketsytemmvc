namespace Saksupermarketsytemmvc.web.Models
{
    public class OrderDetails
    {
        public int OrderDetailId { get; set; }

        public int? OrderId { get; set; }

        public int? ProductId { get; set; }

        public int? Quantity { get; set; }

        public decimal? UnitPrice { get; set; }

        public decimal? TotalPrice { get; set; }

        public virtual Orders? Order { get; set; }

        public virtual Products? Product { get; set; }
    }
}
