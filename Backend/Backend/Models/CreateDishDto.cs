using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class CreateDishDto
    {
        [Required]
        [StringLength(150)]
        [RegularExpression(@"^[a-zA-Z\u00C0-\u017F\s\-\.]+$", ErrorMessage = "Invalid characters")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [StringLength(500)]
        [RegularExpression("^[a-zA-Z\\u00C0-\\u017F\\s\\-\\.0-9,;:()\\[\\]$%&!?*@#+=\\'\\\"]+$", ErrorMessage = "Invalid characters")]
        public string Description { get; set; } = string.Empty;

        [StringLength(500)]
        public string Img { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "RestaurantId must be valid")]
        public int RestaurantId { get; set; }
    }
}
