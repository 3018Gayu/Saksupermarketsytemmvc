using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Saksupermarketsytemmvc.web.Models
{
    [Table("Users")]
    public partial class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string UserName { get; set; } = null!;

        [Required, EmailAddress]
        [Column("User_Email")] // Map to DB column
        [MaxLength(100)]
        public string UserEmail { get; set; } = null!;

        [DataType(DataType.Password)]
        [MaxLength(100)]
        public string? PasswordHash { get; set; } // Nullable for Edit

        [Required]
        [Column("User_Role")] // Map to DB column
        [MaxLength(20)]
        public string UserRole { get; set; } = null!;

        [Required]
        [MaxLength(10)]
        public string? Isactive { get; set; }
    }
}
