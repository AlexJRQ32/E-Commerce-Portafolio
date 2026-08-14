using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Comidas.Models
{
    [Table("PaymentMethods")]
    public class PaymentMethod
    {
        [Key]
        [Column("Id")]
        public string Id { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Column("Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Column("Type")]
        public string Type { get; set; } = string.Empty;

        [StringLength(300)]
        [Column("Description")]
        public string? Description { get; set; } = null;

        [InverseProperty("PaymentMethod")]
        public virtual ICollection<Order>? Orders { get; set; } = new List<Order>();
    }
}