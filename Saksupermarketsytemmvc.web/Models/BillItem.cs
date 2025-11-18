namespace Saksupermarketsytemmvc.web.Models
{
    public class BillItem
    {
        public int BillItemId { get; set; }

        public int BillId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal TotalPrice => Quantity * UnitPrice;

        public Bill? Bill { get; set; }

        public Products? Product { get; set; }
    }
}
