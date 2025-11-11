using System;
using System.Collections.Generic;

namespace Saksupermarketsytemmvc.web.Models;

public partial class OrderDetail
{
    public int Orderdetailsid { get; set; }

    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal? UnitPrice { get; set; }

    public decimal? TotalPrice { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}