using System.ComponentModel.DataAnnotations;

namespace HotelBooker.Models
{
    public class Discount
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        [Required]
        public string? Description { get; set; }

        public DateTime? DiscountStart { get; set; }

        public DateTime? DiscountEnd { get; set; }

        public int? DiscountPercent { get; set; }
    }
}
