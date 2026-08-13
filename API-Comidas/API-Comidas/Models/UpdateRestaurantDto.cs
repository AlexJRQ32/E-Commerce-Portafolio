using System.ComponentModel.DataAnnotations;

namespace API_Comidas.Models
{
    public class UpdateRestaurantDto
    {
        [StringLength(150)]
        public string TradeName { get; set; }

        [StringLength(250)]
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