using System.Threading.Tasks;
using HotelBooker.Models;
using HotelBooker.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooker.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RoomsController : Controller
    {
        private readonly RoomService _roomService;

        public RoomsController(RoomService roomService)
        {
            _roomService = roomService;
        }

        // GET: Rooms
        public async Task<IActionResult> Index()
        {
            var rooms = await _roomService.GetAllRoomsAsync();
            return View(rooms);
        }

        // GET: Rooms/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var room = await _roomService.GetRoomByIdAsync(id.Value);
            if (room == null) return NotFound();

            return View(room);
        }

        // GET: Rooms/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Rooms/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Room room, string[] Furniture)
        {
            if (ModelState.IsValid)
            {
                room.Furniture = string.Join(", ", Furniture);
                await _roomService.CreateRoomAsync(room);
                return RedirectToAction(nameof(Index));
            }
            return View(room);
        }

        // GET: Rooms/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var room = await _roomService.GetRoomByIdAsync(id.Value);
            if (room == null) return NotFound();

            return View(room);
        }

        // POST: Rooms/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Room room)
        {
            if (id != room.Id) return NotFound();

            if (ModelState.IsValid)
            {
                await _roomService.UpdateRoomAsync(room);
                return RedirectToAction(nameof(Index));
            }
            return View(room);
        }

        // GET: Rooms/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var room = await _roomService.GetRoomByIdAsync(id.Value);
            if (room == null) return NotFound();

            return View(room);
        }

        // POST: Rooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _roomService.DeleteRoomAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // GET: HotelRooms
        public async Task<IActionResult> HotelRooms(string SearchString, string PriceRange, string Status)
        {
            var rooms = await _roomService.SearchRoomsAsync(SearchString, PriceRange, Status);
            ViewData["CurrentFilter"] = SearchString;
            return View(rooms);
        }
    }
}
