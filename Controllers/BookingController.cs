using Microsoft.AspNetCore.Mvc;
using HotelBooker.Models;
using HotelBooker.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace HotelBooker.Controllers
{
    public class BookingController : Controller
    {
        private readonly HotelBookerContext _context;
        private readonly UserManager<User> _userManager;

        public BookingController(HotelBookerContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public IActionResult Index(string roomNumber)
        {
            // Lưu thông tin phòng đã chọn vào ViewBag
            ViewBag.SelectedRoom = roomNumber;

            // Chuyển hướng đến view Booking/Index
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Index(int hotelId, string roomNumber)
        {
            var hotel = await _context.Hotel.FindAsync(hotelId);
            var user = await _userManager.GetUserAsync(User);

            if (hotel == null || user == null)
            {
                return NotFound();
            }

            // Thông tin phòng và khách sạn
            var bookingInfo = new
            {
                Hotel = hotel,
                RoomNumber = roomNumber,
                User = user
            };

            return View(bookingInfo);
        }


        // POST: Booking/Complete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(int hotelId, string roomNumber, DateTime checkInDate, DateTime checkOutDate)
        {
            var hotel = await _context.Hotel.FindAsync(hotelId);
            var user = await _userManager.GetUserAsync(User);

            if (hotel == null || user == null)
            {
                return NotFound();
            }

            double totalPrice = CalculateTotalPrice(hotel, checkInDate, checkOutDate);

            // Tạo đơn đặt phòng mới
            var order = new Order
            {
                HotelId = hotelId,
                Hotel = hotel,
                RoomNumber = roomNumber,
                UserId = user.Id,
                User = user,
                BookingDate = DateTime.Now,
                CheckInDate = checkInDate,
                CheckOutDate = checkOutDate,
                TotalPrice = totalPrice
            };

            _context.Add(order);
            await _context.SaveChangesAsync();

            return RedirectToAction("Order", new { orderId = order.Id });
        }

        // GET: Booking/Order
        [HttpGet]
        public async Task<IActionResult> Order(int orderId)
        {
            var order = await _context.Set<Order>()
                .Include(o => o.Hotel)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // Helper function để tính tổng giá
        private double CalculateTotalPrice(Hotel hotel, DateTime checkInDate, DateTime checkOutDate)
        {
            // Ví dụ, bạn có thể tính tổng giá dựa trên số đêm và giá phòng
            var nights = (checkOutDate - checkInDate).Days;
            double pricePerNight = hotel.HotelRate ?? 100; // Giá phòng cơ bản, bạn có thể thay đổi logic này
            return pricePerNight * nights;
        }
    }
}
