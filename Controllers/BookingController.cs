using Microsoft.AspNetCore.Mvc;
using HotelBooker.Models;
using HotelBooker.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HotelBooker.Services; // Add this line

namespace HotelBooker.Controllers
{
    public class BookingController : Controller
    {
        private readonly HotelBookerContext _context;
        private readonly UserManager<User> _userManager;

        private readonly HotelService _hotelService;

        public BookingController(HotelBookerContext context, UserManager<User> userManager, HotelService hotelService)
        {
            _context = context;
            _userManager = userManager;
            _hotelService = hotelService;
        }

        // POST: Booking/Index
        [HttpPost]
        public IActionResult Index(string roomNumber)
        {
            // Lấy thông tin khách sạn từ ViewData
            var hotel = ViewData["Hotel"] as Hotel;
            if (hotel == null)
            {
                return NotFound();
            }

            // Gửi thông tin phòng (roomNumber) và khách sạn đến View
            ViewBag.RoomNumber = roomNumber;
            return View(hotel);
        }
        // GET: Booking/Index
        [HttpGet]
        public async Task<IActionResult> Index(int hotelId, string roomNumber)
        {
            var hotel = await _hotelService.GetHotelByIdAsync(hotelId);
            if (hotel == null)
            {
                return NotFound();
            }

            // Truyền thông tin khách sạn và phòng vào view
            ViewBag.RoomNumber = roomNumber;

            // Không truyền giá vào ViewBag nếu chưa có ngày
            ViewBag.PricePerNight = hotel.HotelRate ?? 100;  // Nếu không có giá, mặc định là 100 USD mỗi đêm
            ViewBag.TotalPrice = 0; // Đặt giá trị tổng là 0 khi chưa có ngày

            return View(hotel);
        }
        // POST: Booking/Complete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(int hotelId, string roomNumber, DateTime checkInDate, DateTime checkOutDate, double totalPrice)
        {
            var hotel = await _context.Hotel.FindAsync(hotelId);
            var user = await _userManager.GetUserAsync(User);
            if (hotel == null || user == null)
            {
                return NotFound();
            }
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
                TotalPrice = totalPrice // Dùng giá trị totalPrice đã được gửi từ form
            };
            // Lưu đơn đặt phòng vào cơ sở dữ liệu
            _context.Add(order);
            await _context.SaveChangesAsync();
            // Chuyển hướng người dùng đến trang Order để xem đơn đặt phòng đã tạo
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
            // Tính số ngày giữa check-in và check-out
            var nights = (checkOutDate - checkInDate).Days;
            // Nếu số ngày là 1, đặt giá mặc định là 100 USD
            if (nights == 1)
            {
                return 100; // Nếu check-in và check-out cùng ngày thì giá là 100 USD
            }
            double pricePerNight = 100; // Nếu không có giá, mặc định là 100 USD mỗi đêm
            return pricePerNight * nights; // Tính tổng số tiền theo số ngày
        }
    }
}
