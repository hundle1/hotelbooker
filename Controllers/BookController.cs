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

        public async Task<IActionResult> Index()
        {
            var rooms = await _roomService.GetAllRoomsAsync(); 
            return View(rooms);
        }


        public IActionResult Order()
        {
            return View();
        }
    }
}
