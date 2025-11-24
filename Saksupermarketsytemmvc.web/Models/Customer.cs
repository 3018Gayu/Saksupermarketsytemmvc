using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Saksupermarketsytemmvc.web.Models
{
    public partial class Customer
    {
        public int CustomerId { get; set; }

        [Column("CUSTOMER_Name")]
        public string CustomerName { get; set; } = null!;

        [Column("CUST_Email")]
        public string? CustEmail { get; set; }

        [Column("CUST_Phone")]
        public string? CustPhone { get; set; }

        [Column("CUST_Address")]
        public string? CustAddress { get; set; }

        public int LoyaltyPoints { get; set; }

        public virtual ICollection<Orders> Orders { get; set; } = new List<Orders>();

        public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();
    }
}
