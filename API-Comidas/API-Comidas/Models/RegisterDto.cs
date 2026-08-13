using System.ComponentModel.DataAnnotations;

namespace API_Comidas.Models
{
    public class RegisterDto
    {
        [Required]
        [StringLength(100)]
        [RegularExpression(@"^[a-zA-Z\u00C0-\u017F\s\-\.]+$", ErrorMessage = "Name can only contain letters, spaces, accents, n-tilde, hyphens, and periods")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(255, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public int RoleId { get; set; } // 2 = Business, 3 = Customer
    }
}
