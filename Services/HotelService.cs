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

        public async Task IncludeUserAsync(Hotel hotel)
        {
            // Tải thông tin User từ bảng User
            if (hotel.UserId != null)
            {
                hotel.User = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == hotel.UserId);
            }
        }
        public async Task<List<Order>> GetOrdersByUserIdAsync(string userId)
        {
            return await _context.Order
                                .Where(o => o.UserId == userId)
                                .Include(o => o.Hotel)
                                .ToListAsync();
        }
        public async Task<bool> DeleteOrderByIdAsync(int orderId)
        {
            try
            {
                // Tìm order theo ID
                var order = await _context.Order.FindAsync(orderId);
                if (order == null)
                {
                    return false; // Không tìm thấy order
                }

                // Xóa order khỏi database
                _context.Order.Remove(order);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false; // Xảy ra lỗi trong quá trình xóa
            }
        }
    }
}
