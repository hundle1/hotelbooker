using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HotelBooker.Models;

namespace HotelBooker.Data
{
    public class HotelBookerContext : DbContext
    {
        public HotelBookerContext(DbContextOptions<HotelBookerContext> options)
            : base(options)
        {
        }

        // Thêm bảng User
        public DbSet<User> Users { get; set; } = default!;

        // Các bảng khác
        public DbSet<Room> Room { get; set; } = default!;
        public DbSet<Hotel> Hotel { get; set; } = default!;

        // Hàm lọc phòng (nếu có)
        internal string? GetFilteredRooms(string searchString, string priceRange, string status)
        {
            throw new NotImplementedException();
        }
    }
}
