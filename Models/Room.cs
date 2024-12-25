using System.ComponentModel.DataAnnotations;
namespace HotelBooker.Models;

public class Room {
    public int Id { get; set; }
    public string? RoomName { get; set; }
    [DataType(DataType.Date)]

    public bool Status { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string? Services { get; set; }
    public decimal Price { get; set; }
}