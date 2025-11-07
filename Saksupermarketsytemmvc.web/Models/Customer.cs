using System;
using System.Collections.Generic;

namespace Saksupermarketsytemmvc.web.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string CustomerName { get; set; } = null!;

    public string? CustEmail { get; set; }

    public string? CustPhone { get; set; }

    public string? CustAddress { get; set; }

    public int LoyaltyPoints { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
