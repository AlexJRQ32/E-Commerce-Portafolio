using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class CreateAddressDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
    }
}
