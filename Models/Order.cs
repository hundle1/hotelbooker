using System.ComponentModel.DataAnnotations;

namespace HotelBooker.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int HotelId { get; set; }
        [StringLength(60, MinimumLength = 3)]
        [Required]
        public required string? CustomerName { get; set; }
        [DataType(DataType.Date)]
        public required DateTime CheckInDate { get; set; }
        public required DateTime CheckOutDate { get; set; }
        public string? ServiceWanted { get; set; }
    }
}
