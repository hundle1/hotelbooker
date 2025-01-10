using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HotelBooker.Models;

namespace HotelBooker.Data
{
    // Kế thừa IdentityDbContext thay vì DbContext để hỗ trợ Identity
    public class HotelBookerContext : IdentityDbContext<User, IdentityRole, string>
    {
        public HotelBookerContext(DbContextOptions<HotelBookerContext> options)
            : base(options)
        {
        }

        // Đảm bảo bạn có DbSet cho User nếu cần
        public DbSet<User> User { get; set; } = default!;
        
        // Các bảng khác
        public DbSet<Room> Room { get; set; } = default!;
        public DbSet<Hotel> Hotel { get; set; } = default!;
        public DbSet<Order> Order { get; set; } = default!;
        
        // Hàm lọc phòng (nếu có)
        internal string? GetFilteredRooms(string searchString, string priceRange, string status)
        {
            throw new NotImplementedException();
        }
    }
}
