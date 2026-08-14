using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    [Table("Coupons")]
    public class Coupon
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Column("Code")]
        public string Code { get; set; }

        [Required]
        [StringLength(100)]
        [Column("Title")]
        public string Title { get; set; }

        [StringLength(500)]
        [Column("Description")]
        public string Description { get; set; }

        [Required]
        [Range(0, 100000)]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Discount { get; set; }

        [Required]
        [Column("IsPercentage")]
        public bool IsPercentage { get; set; }

        [Required]
        [StringLength(10)]
        [Column("ExpirationDate")]
        public string ExpirationDate { get; set; }

        [Required]
        [Column("Active")]
        public bool Active { get; set; }

        [Range(0, 100000)]
        [Column("Stock")]
        public int? Stock { get; set; }

        [Column("RestaurantId")]
        public int? RestaurantId { get; set; }

        [ForeignKey("RestaurantId")]
        public virtual Restaurant? Restaurant { get; set; }

        [Column("UserId")]
        public int? UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}
