using System.ComponentModel.DataAnnotations;

namespace HotelBooker.Models
{
    public class Room
    {
        public int Id { get; set; }

        [StringLength(60, MinimumLength = 3)]
        [Required]
        public string? RoomName { get; set; }
        public bool Status { get; set; }

        [ReleaseDateRequired]
        [DataType(DataType.Date)]
        public DateTime? ReleaseDate { get; set; } 

        public string? Services { get; set; }

        [Range(1, 5000)]
        [Required]
        public decimal Price { get; set; }
        public string? RoomImage { get; set; }

        public string? Furniture { get; set; }

        [Required]
        public string? Description { get; set; }

        public bool Discount { get; set; }

        public DateTime? DiscountStart { get; set; }

        public DateTime? DiscountEnd { get; set; }

        public int? DiscountPercent { get; set; }

        // public int HotelId { get; set; }

        // public required Hotel Hotel { get; set; }

    }
}
