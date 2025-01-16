using System;
using System.ComponentModel.DataAnnotations;

namespace HotelBooker.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public int HotelId { get; set; }
        public Hotel? Hotel { get; set; }

        [Required]
        public string? RoomNumber { get; set; }

        [Required]
        public string? UserId { get; set; }
        public User? User { get; set; }

        [Required]
        public DateTime BookingDate { get; set; }

        [Required]
        public DateTime CheckInDate { get; set; }

        [Required]
        public DateTime CheckOutDate { get; set; }

        [Required]
        public double TotalPrice { get; set; }

        public bool Status { get; set; }
    }
}
