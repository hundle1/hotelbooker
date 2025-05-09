using System.ComponentModel.DataAnnotations;

namespace HotelBooker.Models
{
    public class Hotel
    {
        public int Id { get; set; }
        [StringLength(60, MinimumLength = 3)]
        [Required]
        public required string? HotelName { get; set; }
        public string? HotelImage { get; set; }
        public string? HotelDescription { get; set; }
        public string? HotelLocation { get; set; }
        [Range(1, 5)]
        public int? HotelRate { get; set; }

        [Range(1, 2000)]
        public int? NumberOfRoom { get; set; }
        public string? HotelServices { get; set; }
        public string? HotelContact { get; set; }
        public string? HotelEmail { get; set; }
        public string? HotelWebsite { get; set; }
        
        public string? UserId { get; set; }
        public User? User { get; set; } 


    }
}
