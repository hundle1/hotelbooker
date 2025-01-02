using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelBooker.Data;
using HotelBooker.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBooker.Services
{
    public class HotelService
    {
        private readonly HotelBookerContext _context;

        public HotelService(HotelBookerContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Hotel>> GetAllHotelsAsync()
        {
            return await _context.Hotel.ToListAsync();
        }

        public async Task<Hotel?> GetHotelByIdAsync(int id)
        {
            return await _context.Hotel.FindAsync(id);
        }

        public async Task CreateHotelAsync(Hotel hotel)
        {
            _context.Hotel.Add(hotel);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateHotelAsync(Hotel hotel)
        {
            _context.Hotel.Update(hotel);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteHotelAsync(int id)
        {
            var hotel = await _context.Hotel.FindAsync(id);
            if (hotel != null)
            {
                _context.Hotel.Remove(hotel);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Hotel>> SearchHotelAsync(string searchString, int? minRate, int? maxRate, string location)
        {
            var query = _context.Hotel.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
                query = query.Where(h => h.HotelName != null && h.HotelName.Contains(searchString));

            if (minRate.HasValue)
                query = query.Where(h => h.HotelRate >= minRate.Value);

            if (maxRate.HasValue)
                query = query.Where(h => h.HotelRate <= maxRate.Value);

            if (!string.IsNullOrEmpty(location))
                query = query.Where(h => h.HotelLocation != null && h.HotelLocation.Contains(location));

            return await query.ToListAsync();
        }

        public async Task<bool> HotelExistsAsync(int id)
        {
            return await _context.Hotel.AnyAsync(h => h.Id == id);
        }
    }
}
