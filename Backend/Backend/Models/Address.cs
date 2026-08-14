using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    [Table("Addresses")]
    public class Address
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

        [Required]
        [StringLength(200)]
        [Column("Street")]
        public string Street { get; set; }

        [StringLength(100)]
        [Column("City")]
        public string City { get; set; }

        [StringLength(20)]
        [Column("PostalCode")]
        public string PostalCode { get; set; }

        [Column("Latitude")]
        public double? Latitude { get; set; }

        [Column("Longitude")]
        public double? Longitude { get; set; }

        [Required]
        [Column("UserId")]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}
