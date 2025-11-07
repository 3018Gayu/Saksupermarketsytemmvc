using System;
using System.Collections.Generic;

namespace Saksupermarketsytemmvc.web.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public string? InvoiceNo { get; set; }

    public int CustomerId { get; set; }

    public DateTime? OrderDate { get; set; }

    public decimal? TotalAmount { get; set; }

    public decimal? TaxAmount { get; set; }

    public decimal? Discount { get; set; }

    public decimal? NetAmount { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
