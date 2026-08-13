using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Comidas.Models
{
    [Table("OrderItems")]
    public class OrderItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Column("OrderId")]
        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public virtual Order? Order { get; set; }

        [Required]
        [Column("DishId")]
        public int DishId { get; set; }

        [ForeignKey("DishId")]
        public virtual Dish? Dish { get; set; }

        [Range(1, 1000)]
        [Column("Quantity")]
        public int Quantity { get; set; }

        [StringLength(150)]
        [Column("Name")]
        public string Name { get; set; } = string.Empty;

        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }
    }
}