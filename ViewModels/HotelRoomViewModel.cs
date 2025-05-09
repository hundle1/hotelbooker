using System.ComponentModel.DataAnnotations;

namespace HotelBooker.Models.ViewModels
{
    public class HotelRoomViewModel
    {
        public Hotel Hotel { get; set; } = new Hotel { HotelName = "Default Hotel Name" };
        public Room Room { get; set; } = new Room();
    }
}
