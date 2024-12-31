using System.ComponentModel.DataAnnotations;

namespace HotelBooker.Models
{
    public class Account
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        [Required]
        public DateTime? Birth { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }

        public string? Role { get; set; }

        public string? Address { get; set; }

        public string? Phone { get; set; }

        public string? Image { get; set; }

        public string? Gender { get; set; }
    }
}
