using HotelBooker.Services;
using Microsoft.AspNetCore.Mvc;
using HotelBooker.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;

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
            // 1. Lấy thông tin hotel
            var hotel = await _hotelService.GetHotelByIdAsync(id);
            if (hotel == null)
                return NotFound();

            // 2. (Nếu cần) load thêm thông tin User của hotel
            await _hotelService.IncludeUserAsync(hotel);

            // 3. Lấy userId hiện tại
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null)
            {
                // 4. Lấy tất cả orders của user
                var userOrders = await _hotelService.GetOrdersByUserIdAsync(userId);
                // 5. Lọc chỉ những order cùng hotel này
                var ordersForThisHotel = userOrders
                    .Where(o => o.HotelId == id)
                    .ToList();
                ViewData["Orders"] = ordersForThisHotel;
            }
            else
            {
                ViewData["Orders"] = new List<Order>();
            }

            // 6. Trả về view với model là hotel
            return View(hotel);
        }
    }
}
