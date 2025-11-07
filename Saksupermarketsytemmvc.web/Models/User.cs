using System;
using System.Collections.Generic;

namespace Saksupermarketsytemmvc.web.Models;

public partial class User
{
    public int UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string UserEmail { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string UserRole { get; set; } = null!;

    public string? Isactive { get; set; }
}
