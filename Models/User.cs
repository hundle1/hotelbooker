using System.ComponentModel.DataAnnotations;

namespace HotelBooker.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string? UserName { get; set; }

        public string? Address { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        public DateTime? Birth { get; set; }

        [Required]
        public string? Role { get; set; } = "User";
        public string? Image { get; set; } = null;
        public string? Gender { get; set; } = null;

        public UserStatus Status { get; set; } = UserStatus.Active;
    }
    public enum UserStatus
    {
        Active,
        Inactive
    }

}

