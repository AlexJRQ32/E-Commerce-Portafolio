using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    [Table("Roles")]
    public class Role
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Column("Name")]
        public string Name { get; set; }

        [StringLength(200)]
        [Column("Subtitle")]
        public string Subtitle { get; set; }

        [StringLength(200)]
        [Column("Site")]
        public string Site { get; set; }

        [StringLength(50)]
        [Column("Icon")]
        public string Icon { get; set; }

        [InverseProperty("Role")]
        public virtual ICollection<User>? Users { get; set; } = new List<User>();
    }
}
