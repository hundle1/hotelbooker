using System.ComponentModel.DataAnnotations;

namespace HotelBooker.Models
{
    public class Room
    {
        public int Id { get; set; }
        public string? RoomName { get; set; }

        public bool Status { get; set; }

        [ReleaseDateRequired]
        [DataType(DataType.Date)]
        public DateTime? ReleaseDate { get; set; } 

        public string? Services { get; set; }
        public decimal Price { get; set; }
        public string? RoomImage { get; set; }

        public string? Furniture { get; set; }

        public string? Description { get; set; }
    }
}
