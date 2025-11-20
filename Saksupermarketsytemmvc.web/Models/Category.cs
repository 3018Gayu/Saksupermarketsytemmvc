using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Saksupermarketsytemmvc.web.Models
{
    public partial class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Category Name is required")]
        [StringLength(100)]
        public string CategoryName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        // Navigation Property (never null)
        public virtual ICollection<Products> Products { get; set; } = new List<Products>();
    }
}
