using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    [Table("Restaurants")]
    public class Restaurant
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        [Required]
        [StringLength(150)]
        [Column("TradeName")]
        public string TradeName { get; set; }

        [Required]
        [StringLength(250)]
        [Column("Address")]
        public string Address { get; set; }

        [Required]
        [Column("CategoryId")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }

        [Required]
        [Column("UserId")]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [Required]
        [Column("OpeningTime")]
        public TimeOnly OpeningTime { get; set; }

        [Required]
        [Column("ClosingTime")]
        public TimeOnly ClosingTime { get; set; }

        [StringLength(500)]
        [Column("Img")]
        public string Img { get; set; }

        [Column(TypeName = "decimal(3,2)")]
        public decimal Rating { get; set; }

        [Column("IsOpen")]
        public bool IsOpen { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal DeliveryFee { get; set; }

        [StringLength(50)]
        [Column("DeliveryTime")]
        public string DeliveryTime { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal MinOrderAmount { get; set; }

        [InverseProperty("Restaurant")]
        public virtual ICollection<Dish>? Dishes { get; set; } = new List<Dish>();

        [InverseProperty("RestaurantRef")]
        public virtual ICollection<Order>? Orders { get; set; } = new List<Order>();

        [InverseProperty("Restaurant")]
        public virtual ICollection<Coupon>? Coupons { get; set; } = new List<Coupon>();
    }
}
