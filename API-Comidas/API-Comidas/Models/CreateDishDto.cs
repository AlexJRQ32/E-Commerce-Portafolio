using System.ComponentModel.DataAnnotations;

namespace API_Comidas.Models
{
    public class CreateDishDto
    {
        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [StringLength(500)]
        public string Img { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "RestaurantId must be valid")]
        public int RestaurantId { get; set; }
    }
}
