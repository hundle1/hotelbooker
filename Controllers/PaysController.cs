using HotelBooker.ViewModels.VNPay;
using HotelBooker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using HotelBooker.Models;
using Microsoft.EntityFrameworkCore;
using HotelBooker.Data;

namespace HotelBooker.Controllers
{
    public class PaysController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IVNPayService _vnPayService;

        private readonly HotelBookerContext _dbContext;
        public PaysController(IVNPayService vnPayService, UserManager<User> userManager, HotelBookerContext dbContext)
        {
            _vnPayService = vnPayService;
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public static Dictionary<string, string> vnp_TransactionStatus = new Dictionary<string, string>()
        {
            {"00","Transaction successful" },
            {"01","Transaction not completed" },
            {"02","Transaction failed" },
            {"04","Transaction is being rolled back" },
            {"05","VNPAY is processing the transaction" },
            {"06","VNPAY has sent a refund request to the bank" },
            {"07","Transaction is suspected of fraud" },
            {"09","Refund failed" }
        };

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Pay(double? amount)
        {
            var currentUser = _userManager.GetUserAsync(User).Result;
            if (currentUser == null)
            {
                return RedirectToAction("Index", "Home");
            }
            double? amountInVND = amount.HasValue ? amount.Value * 24500 : (double?)null;
            var model = new CheckoutViewModel
            {
                FullName = currentUser.UserName,
                Address = currentUser.Email,
                PhoneNumber = currentUser.PhoneNumber,
                Amount = amountInVND?.ToString("N0")
            };
            return View(model);
        }


        [HttpPost]
        public IActionResult Pay(CheckoutViewModel request)
        {
            if (ModelState.IsValid)
            {
                if (request.PaymentMethod == "VNPay")
                {
                    string cleanedAmount = request.Amount!.Replace(".", "");
                    var vnPayModel = new VNPaymentRequestModel
                    {
                        // Fill Amount 
                        Amount = Double.Parse(cleanedAmount),
                        CreatedDate = DateTime.Now,
                        Description = $"{request.FullName} {request.PhoneNumber}",
                        FullName = request.FullName,
                        OrderId = new Random().Next(1000, 100000)
                    };
                    return Redirect(_vnPayService.CreatePaymentUrl(HttpContext, vnPayModel));
                }

                // Processed

                return View();
            }
            return View(request);
        }

        public IActionResult PaymentSuccess(string orderId)
        {
            // Chuyển orderId từ string sang long (hoặc int tùy kiểu dữ liệu của bạn)
            if (long.TryParse(orderId, out long parsedOrderId))
            {
                // Lấy thông tin đơn hàng từ cơ sở dữ liệu theo Id
                var order = _dbContext.Order.FirstOrDefault(o => o.Id == parsedOrderId);
                
                if (order != null)
                {
                    // Kiểm tra và cập nhật trạng thái đơn hàng
                    order.Status = true;  // Đặt trạng thái thành 'đã thanh toán'
                    
                    // Lưu thay đổi vào cơ sở dữ liệu
                    _dbContext.SaveChanges();  
                }
                else
                {
                    // Nếu không tìm thấy đơn hàng trong cơ sở dữ liệu
                    ViewBag.ErrorMessage = "Order not found.";
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid order ID.";
            }

            return View();
        }


        public IActionResult PaymentFail()
        {
            return View();
        }
        public IActionResult PaymentCallBack()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            // Kiểm tra mã phản hồi của VNPay
            if (response.VNPayResponseCode == "00")
            {
                // Lấy orderId từ response hoặc session
                var orderId = response.OrderId?.ToString();

                // Debug: Kiểm tra orderId trước khi chuyển hướng
                Console.WriteLine($"Payment successful. Redirecting to PaymentSuccess with OrderId: {orderId}");

                return RedirectToAction(nameof(PaymentSuccess), new { orderId = orderId });
            }

            // Nếu có lỗi, hiển thị thông báo lỗi
            if (vnp_TransactionStatus.TryGetValue(response.VNPayResponseCode!, out var message))
            {
                TempData["Message"] = $"Payment error: {message}";
            }
            else
            {
                TempData["Message"] = $"Unknown payment error: {response.VNPayResponseCode}";
            }
            return RedirectToAction(nameof(PaymentFail));
        }
    }
}