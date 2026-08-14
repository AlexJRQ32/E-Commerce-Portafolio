using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    [Table("Categories")]
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        [Required]
        [StringLength(100)]
        [Column("Name")]
        public string Name { get; set; }

        [StringLength(50)]
        [Column("Icon")]
        public string Icon { get; set; }

        [StringLength(50)]
        [Column("Slug")]
        public string Slug { get; set; }

        [InverseProperty("Category")]
        public virtual ICollection<Restaurant>? Restaurants { get; set; } = new List<Restaurant>();
    }
}
