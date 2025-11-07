using System;
using System.Collections.Generic;

namespace Saksupermarketsytemmvc.web.Models;

public partial class Supplier
{
    public int SupplierId { get; set; }

    public string Name { get; set; } = null!;

    public string Contact { get; set; } = null!;

    public string Address { get; set; } = null!;
}
