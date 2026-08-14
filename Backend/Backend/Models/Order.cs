using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    [Table("Orders")]
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public enum OrderStatus
        {
            Pending,
            Confirmed,
            Preparing,
            Ready,
            OutForDelivery,
            Delivered,
            Cancelled
        }

        [Required]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        [StringLength(50)]
        public string? CouponCodeApplied { get; set; }

        [Column("CustomerId")]
        public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public virtual User? Customer { get; set; }

        [Column("PaymentMethodId")]
        public int PaymentMethodId { get; set; }

        [ForeignKey("PaymentMethodId")]
        public virtual PaymentMethod? PaymentMethod { get; set; }

        [Column("AddressId")]
        public int AddressId { get; set; }

        [ForeignKey("AddressId")]
        public virtual Address? Address { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Subtotal { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Tax { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal DeliveryFee { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Total { get; set; }

        [Column("RestaurantId")]
        public int RestaurantId { get; set; }

        [ForeignKey("RestaurantId")]
        public virtual Restaurant? RestaurantRef { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        [StringLength(200)]
        public string? CancellationReason { get; set; }

        [InverseProperty("Order")]
        public List<OrderItem>? Items { get; set; } = new List<OrderItem>();
    }
}
