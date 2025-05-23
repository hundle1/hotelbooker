using HotelBooker.Data;
using HotelBooker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelBooker.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrdersController : Controller
    {
        private readonly HotelBookerContext _context;

        public OrdersController(HotelBookerContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            var orders = _context.Order
                .Include(o => o.Hotel)
                .Include(o => o.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                orders = orders.Where(o => (o.Hotel != null && o.Hotel.HotelName != null && o.Hotel.HotelName.Contains(searchString))
                                        || (o.RoomNumber != null && o.RoomNumber.Contains(searchString))
                                        || (o.User != null && o.User.UserName != null && o.User.UserName.Contains(searchString))
                                        || o.BookingDate.ToString().Contains(searchString)
                                        || o.TotalPrice.ToString().Contains(searchString));
            }

            switch (sortOrder)
            {
                case "date-asc":
                    orders = orders.OrderBy(o => o.BookingDate);
                    break;
                case "date-desc":
                    orders = orders.OrderByDescending(o => o.BookingDate);
                    break;
                case "price-asc":
                    orders = orders.OrderBy(o => o.TotalPrice);
                    break;
                case "price-desc":
                    orders = orders.OrderByDescending(o => o.TotalPrice);
                    break;
                default:
                    break;
            }

            return View(await orders.ToListAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Order
                .Include(o => o.Hotel)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var order = await _context.Order
                .Include(o => o.Hotel)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Order.FindAsync(id);
            if (order != null)
            {
                _context.Order.Remove(order);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}