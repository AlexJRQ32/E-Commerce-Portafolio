using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Comidas.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Column("Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(150)]
        [Column("Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        [Column("Password")]
        public string Password { get; set; } = string.Empty;

        [StringLength(50)]
        [Column("Phone")]
        public string? Phone { get; set; }

        [StringLength(500)]
        [Column("Img")]
        public string? Img { get; set; }

        [Required]
        [Column("RoleId")]
        public int RoleId { get; set; }

        [ForeignKey("RoleId")]
        public virtual Role? Role { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<Address>? Addresses { get; set; } = new List<Address>();

        [InverseProperty("User")]
        public virtual ICollection<Restaurant>? Restaurants { get; set; } = new List<Restaurant>();

        [InverseProperty("Customer")]
        public virtual ICollection<Order>? Orders { get; set; } = new List<Order>();

        [InverseProperty("User")]
        public virtual ICollection<Coupon>? Coupons { get; set; } = new List<Coupon>();
    }
}