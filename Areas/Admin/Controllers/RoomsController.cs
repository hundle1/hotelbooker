using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotelBooker.Data;
using HotelBooker.Models;

namespace HotelBooker.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RoomsController : Controller
    {
        private readonly HotelBookerContext _context;

        public RoomsController(HotelBookerContext context)
        {
            _context = context;
        }

        // GET: Rooms
        public IActionResult Index(string SearchString, string PriceRange, string Status)
        {
            var rooms = _context.Room.AsQueryable();
            if (!string.IsNullOrEmpty(SearchString))
            {
                rooms = rooms.Where(r => r.RoomName.Contains(SearchString));
            }
            if (!string.IsNullOrEmpty(PriceRange))
            {
                switch (PriceRange)
                {
                    case "1":
                        rooms = rooms.Where(r => r.Price >= 0 && r.Price <= 500);
                        break;
                    case "2":
                        rooms = rooms.Where(r => r.Price > 500 && r.Price <= 1000);
                        break;
                    case "3":
                        rooms = rooms.Where(r => r.Price > 1000);
                        break;
                }
            }
            // Lọc theo trạng thái (Status)
            if (!string.IsNullOrEmpty(Status))
            {
                var isAvailable = Status == "true";
                rooms = rooms.Where(r => r.Status == isAvailable);
            }

            // Truyền giá trị của `SearchString` vào ViewData để giữ trạng thái input
            ViewData["CurrentFilter"] = SearchString;

            return View(rooms.ToList());
        }

        // GET: Rooms/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Room
                .FirstOrDefaultAsync(m => m.Id == id);
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Room room, string[] Furniture)
        {
            if (ModelState.IsValid)
            {
                room.Furniture = string.Join(", ", Furniture);
                _context.Add(room);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(room);
        }



        // GET: Rooms/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Room.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }
            return View(room);
        }

        // POST: Rooms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RoomName,Status,ReleaseDate,Services,Price")] Room room)
        {
            if (id != room.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(room);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoomExists(room.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(room);
        }

        // GET: Rooms/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Room
                .FirstOrDefaultAsync(m => m.Id == id);
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }

        // POST: Rooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await _context.Room.FindAsync(id);
            if (room != null)
            {
                _context.Room.Remove(room);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RoomExists(int id)
        {
            return _context.Room.Any(e => e.Id == id);
        }
    }
}
