using System;
using System.Collections.Generic;

namespace Saksupermarketsytemmvc.web.Models;

public partial class InventoryTransaction
{
    public int TransId { get; set; }

    public int ProductId { get; set; }

    public int? Quantity { get; set; }

    public string? TransType { get; set; }

    public string? Remarks { get; set; }

    public DateTime? TransDate { get; set; }

    public virtual Product Product { get; set; } = null!;
}