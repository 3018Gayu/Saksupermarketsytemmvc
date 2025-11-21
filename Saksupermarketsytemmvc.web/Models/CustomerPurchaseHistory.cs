namespace Saksupermarketsytemmvc.web.Models
{
    public class CustomerPurchaseHistory
    {
        public Customer Customer { get; set; }
        public List<Bill> Bills { get; set; }
    }
}
