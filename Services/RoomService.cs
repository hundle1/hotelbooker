using HotelBooker.Data;
using HotelBooker.Models;
using System.Collections.Generic;
using System.Linq;

namespace HotelBooker.Services
{
    public class RoomService
    {
        private readonly HotelBookerContext _context;

        public RoomService(HotelBookerContext context)
        {
            _context = context;
        }

        public IEnumerable<Room> GetAllRooms()
        {
            return _context.Room.ToList();
        }

        public Room GetRoomById(int id)
        {
#pragma warning disable CS8603
            return _context.Room.FirstOrDefault(r => r.Id == id);
#pragma warning restore CS8603
        }
    }
}
