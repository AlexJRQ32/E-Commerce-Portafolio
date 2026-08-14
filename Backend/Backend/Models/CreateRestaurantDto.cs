using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class CreateRestaurantDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(150)]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñ��üÜ\s\-\.]+$", ErrorMessage = "Invalid characters")]
        public string TradeName { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public TimeOnly OpeningTime { get; set; } = new TimeOnly(8, 0);

        [Required]
        public TimeOnly ClosingTime { get; set; } = new TimeOnly(22, 0);

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal DeliveryFee { get; set; }

        public string DeliveryTime { get; set; } = "30-45 min";

        [Column(TypeName = "decimal(10,2)")]
        public decimal MinOrderAmount { get; set; }
    }
}
