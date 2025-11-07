using System;
using System.Collections.Generic;

namespace Saksupermarketsytemmvc.web.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string Name { get; set; } = null!;

    public int CategoryId { get; set; }

    public decimal UnitPrice { get; set; }

    public int? StockQty { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
