using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Comidas.Models
{
    [Table("Orders")]
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(150)]
        [Column("Restaurant")]
        public string Restaurant { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Column("Status")]
        public string Status { get; set; } = "Pending";

        [Required]
        [StringLength(10)]
        [Column("Date")]
        public string Date { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        [Column("Time")]
        public string Time { get; set; } = string.Empty;

        [StringLength(50)]
        [Column("CouponCodeApplied")]
        public string? CouponCodeApplied { get; set; }

        [Required]
        [Column("CustomerId")]
        public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public virtual User? Customer { get; set; }

        [StringLength(50)]
        [Column("PaymentMethodId")]
        public string PaymentMethodId { get; set; } = string.Empty;

        [ForeignKey("PaymentMethodId")]
        public virtual PaymentMethod? PaymentMethod { get; set; }

        [StringLength(100)]
        [Column("AddressId")]
        public string AddressId { get; set; } = string.Empty;

        [ForeignKey("AddressId")]
        public virtual Address? Address { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Total { get; set; }

        [Required]
        [Column("RestaurantId")]
        public int RestaurantId { get; set; }

        [ForeignKey("RestaurantId")]
        public virtual Restaurant? RestaurantRef { get; set; }

        [InverseProperty("Order")]
        public List<OrderItem>? Items { get; set; } = new List<OrderItem>();
    }
}