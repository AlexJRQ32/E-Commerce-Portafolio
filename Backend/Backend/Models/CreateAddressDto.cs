using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class CreateAddressDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Street { get; set; } = string.Empty;

        [StringLength(100)]
        public string City { get; set; }

        [StringLength(20)]
        public string PostalCode { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }
    }
}
