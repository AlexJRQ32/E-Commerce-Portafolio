using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    [Table("Addresses")]
    public class Address
    {
        [Key]
        [Column("Id")]
        public string Id { get; set; }

        [Required]
        [StringLength(200)]
        [Column("Name")]
        public string Name { get; set; }

        [Required]
        [Column("UserId")]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}
