using HotelBooker.Models;
using HotelBooker.Models.ViewModels; // <- thêm dòng này
using HotelBooker.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooker.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HotelsController : Controller
    {
        private readonly HotelService _hotelService;

        public HotelsController(HotelService hotelService)
        {
            _hotelService = hotelService;
        }

        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            var hotels = await _hotelService.GetAllHotelsAsync();

            if (!string.IsNullOrEmpty(searchString))
            {
                hotels = hotels.Where(h => (h.HotelName != null && h.HotelName.Contains(searchString))
                                        || (h.HotelRate?.ToString() != null && h.HotelRate.Value.ToString().Contains(searchString))
                                        || (h.NumberOfRoom?.ToString() != null && h.NumberOfRoom.Value.ToString().Contains(searchString))).ToList();
            }

            switch (sortOrder)
            {
                case "name-asc":
                    hotels = hotels.OrderBy(h => h.HotelName).ToList();
                    break;
                case "name-desc":
                    hotels = hotels.OrderByDescending(h => h.HotelName).ToList();
                    break;
                case "rate-asc":
                    hotels = hotels.OrderBy(h => h.HotelRate).ToList();
                    break;
                case "rate-desc":
                    hotels = hotels.OrderByDescending(h => h.HotelRate).ToList();
                    break;
                case "room-asc":
                    hotels = hotels.OrderBy(h => h.NumberOfRoom).ToList();
                    break;
                case "room-desc":
                    hotels = hotels.OrderByDescending(h => h.NumberOfRoom).ToList();
                    break;
                default:
                    break;
            }

            return View(hotels);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Hotel hotel)
        {
            if (ModelState.IsValid)
            {
                _ = _hotelService.CreateHotelAsync(hotel);
                return RedirectToAction(nameof(Index));
            }
            return View(hotel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var hotel = await _hotelService.GetHotelByIdAsync(id);
            if (hotel == null)
            {
                return NotFound();
            }
            return View(hotel);
        }

        public IActionResult Edit(int id)
        {
            var hotel = _hotelService.GetHotelByIdAsync(id).Result;
            if (hotel == null)
            {
                return NotFound();
            }
            return View(hotel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Hotel hotel)
        {
            if (id != hotel.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                _ = _hotelService.UpdateHotelAsync(hotel);
                return RedirectToAction(nameof(Index));
            }
            return View(hotel);
        }

        public IActionResult Delete(int id)
        {
            var hotel = _hotelService.GetHotelByIdAsync(id).Result;
            if (hotel == null)
            {
                return NotFound();
            }
            return View(hotel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _ = _hotelService.DeleteHotelAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
