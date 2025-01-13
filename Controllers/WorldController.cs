using HotelBooker.Services;
using Microsoft.AspNetCore.Mvc;
using HotelBooker.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBooker.Controllers
{
    public class WorldController : Controller
    {
        private readonly HotelService _hotelService;

        public WorldController(HotelService hotelService)
        {
            _hotelService = hotelService;
        }

        public async Task<IActionResult> Index()
        {
            var hotels = await _hotelService.GetAllHotelsAsync();
            var continentHotelCount = new Dictionary<string, int>
            {
                { "Asia", 0 },
                { "Europe", 0 },
                { "Africa", 0 },
                { "America", 0 },
                { "Australia", 0 }
            };
            foreach (var hotel in hotels)
            {
                if (hotel.HotelLocation != null && continentHotelCount.ContainsKey(hotel.HotelLocation))
                {
                    continentHotelCount[hotel.HotelLocation]++;
                }
            }
            ViewBag.ContinentHotelCount = continentHotelCount;
            return View(hotels);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var hotel = await _hotelService.GetHotelByIdAsync(id);
            if (hotel == null)
            {
                return NotFound();
            }

            // Đảm bảo rằng khi lấy khách sạn, thông tin người dùng cũng được tải
            await _hotelService.IncludeUserAsync(hotel); // Giả sử có phương thức này để tải User từ bảng User

            ViewData["Hotel"] = hotel;
            return View(hotel);
        }
    }
}
