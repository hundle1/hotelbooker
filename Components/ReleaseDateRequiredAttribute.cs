using System.ComponentModel.DataAnnotations;
using HotelBooker.Models;

public class ReleaseDateRequiredAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var room = (Room)validationContext.ObjectInstance;

        if (room.Status && value == null)
        {
            return new ValidationResult("ReleaseDate is required when the room is In Used.");
        }

        return ValidationResult.Success;
    }
}
