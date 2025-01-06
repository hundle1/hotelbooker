using System.ComponentModel.DataAnnotations;

namespace HotelBooker.Models
{
    public class Account
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }

        public string? Role { get; set; } = "User";

        public DateTime? Birth { get; set; } = null; 
        public string? Address { get; set; } = null;
        public string? Phone { get; set; } = null;
        public string? Image { get; set; } = null;
        public string? Gender { get; set; } = null;
    }
}
