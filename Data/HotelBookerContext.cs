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
        public HotelBookerContext (DbContextOptions<HotelBookerContext> options)
            : base(options)
        {
        }

        public DbSet<HotelBooker.Models.Room> Room { get; set; } = default!;

        public DbSet<HotelBooker.Models.Hotel> Hotel { get; set; } = default!;


        internal string? GetFilteredRooms(string searchString, string priceRange, string status)
        {
            throw new NotImplementedException();
        }
    }
}
