using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class UpdateRestaurantDto
    {
        [StringLength(150)]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñ��üÜ\s\-\.]+$", ErrorMessage = "Invalid characters")]
        public string TradeName { get; set; }

        [StringLength(250)]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñ��üÜ\s\-\.0-9#,]+$", ErrorMessage = "Invalid characters")]
        public string Address { get; set; }

        public int CategoryId { get; set; }

        public TimeOnly? OpeningTime { get; set; }

        public TimeOnly? ClosingTime { get; set; }

        [StringLength(500)]
        public string Img { get; set; }

        public bool? IsOpen { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? DeliveryFee { get; set; }

        public string DeliveryTime { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? MinOrderAmount { get; set; }
    }
}
