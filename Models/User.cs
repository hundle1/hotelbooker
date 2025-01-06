using System.ComponentModel.DataAnnotations;

public class User
{
    public int UserId { get; set; }

    [Required]
    public string? UserName { get; set; }

    [Required]
    public string? Address { get; set; }

    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }

    [DataType(DataType.PhoneNumber)]
    public string? Phone { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    [Required]
    public string? Role { get; set; }

    public UserStatus Status { get; set; } = UserStatus.Active;
    public void HashPassword()
    {
        Password = BCrypt.Net.BCrypt.HashPassword(Password);
    }
}

public enum UserStatus
{
    Active,
    Inactive
}
