using HotelBooker.Models;
using HotelBooker.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooker.Controllers
{
    public class BookController : Controller
    {
        private readonly RoomService _roomService;

        public BookController(RoomService roomService)
        {
            _roomService = roomService;
        }

        public IActionResult Index()
        {
            var rooms = _roomService.GetAllRooms();
            return View(rooms);
        }

        public IActionResult Order()
        {
            return View();
        }
    }
}
