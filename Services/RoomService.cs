using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelBooker.Data;
using HotelBooker.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBooker.Services
{
    public class RoomService
    {
        private readonly HotelBookerContext _context;

        public RoomService(HotelBookerContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Room>> GetAllRoomsAsync()
        {
            return await _context.Room.ToListAsync();
        }

        public async Task<Room> GetRoomByIdAsync(int id)
        {
            return await _context.Room.FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task CreateRoomAsync(Room room)
        {
            _context.Room.Add(room);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRoomAsync(Room room)
        {
            _context.Room.Update(room);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRoomAsync(int id)
        {
            var room = await _context.Room.FindAsync(id);
            if (room != null)
            {
                _context.Room.Remove(room);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Room>> SearchRoomsAsync(string searchString, string priceRange, string status)
        {
            var query = _context.Room.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
                query = query.Where(r => r.RoomName.Contains(searchString));

            if (!string.IsNullOrEmpty(priceRange))
            {
                switch (priceRange)
                {
                    case "1":
                        query = query.Where(r => r.Price >= 0 && r.Price <= 500);
                        break;
                    case "2":
                        query = query.Where(r => r.Price > 500 && r.Price <= 1000);
                        break;
                    case "3":
                        query = query.Where(r => r.Price > 1000);
                        break;
                }
            }

            if (!string.IsNullOrEmpty(status))
            {
                var isAvailable = status == "true";
                query = query.Where(r => r.Status == isAvailable);
            }

            return await query.ToListAsync();
        }

        public async Task<bool> RoomExistsAsync(int id)
        {
            return await _context.Room.AnyAsync(r => r.Id == id);
        }
    }
}
