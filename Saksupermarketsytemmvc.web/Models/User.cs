using System.ComponentModel.DataAnnotations;

namespace Saksupermarketsytemmvc.web.Models;

public partial class User
{
    public int UserId { get; set; }

    [Required]
    public string UserName { get; set; } = null!;

    [Required, EmailAddress]
    public string UserEmail { get; set; } = null!;

    [DataType(DataType.Password)]
    public string? PasswordHash { get; set; } // Made nullable for Edit

    [Required]
    public string UserRole { get; set; } = null!;

    [Required]
    public string? Isactive { get; set; }
}
