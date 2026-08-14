using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class UpdateRestaurantDto
    {
        [StringLength(150)]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s\-\.]+$", ErrorMessage = "Invalid characters")]
        public string TradeName { get; set; }

        [StringLength(250)]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s\-\.0-9#,]+$", ErrorMessage = "Invalid characters")]
        public string Address { get; set; }

        public int CategoryId { get; set; }

        [StringLength(10)]
        public string OpeningTime { get; set; }

        [StringLength(10)]
        public string ClosingTime { get; set; }

        [StringLength(500)]
        public string Img { get; set; }

        public bool? IsOpen { get; set; }
    }
}
