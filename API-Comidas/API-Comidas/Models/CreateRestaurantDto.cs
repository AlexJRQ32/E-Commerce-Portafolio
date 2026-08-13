using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Comidas.Models
{
    public class CreateRestaurantDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(150)]
        public string TradeName { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [StringLength(10)]
        public string OpeningTime { get; set; } = "08:00";

        [StringLength(10)]
        public string ClosingTime { get; set; } = "22:00";
    }
}