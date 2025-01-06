using HotelBooker.Data;
using HotelBooker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

public class AccountController : Controller
{
    private readonly HotelBookerContext _context;

    public AccountController(HotelBookerContext context)
    {
        _context = context;
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        // Tìm người dùng theo email
        var user = _context.Users.FirstOrDefault(u => u.Email == email);

        // Kiểm tra thông tin tài khoản
        if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))
        {
            // Tạo claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty, ClaimValueTypes.String),
                new Claim(ClaimTypes.Role, user.Role! ?? string.Empty, ClaimValueTypes.String)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties { IsPersistent = true };

            // Đăng nhập người dùng
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties
            );

            return RedirectToAction("Index", "Home");
        }

        // Thông báo lỗi
        ViewBag.Error = "Invalid email or password.";
        return View();
    }

    // GET: /Account/SignUp
    [HttpGet]
    public IActionResult SignUp()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SignUp(Account model, string confirmPassword)
    {
        // Kiểm tra tính hợp lệ của dữ liệu đầu vào
        if (ModelState.IsValid)
        {
            // Kiểm tra nếu mật khẩu và xác nhận mật khẩu không khớp
            if (model.Password != confirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Passwords do not match.");
                return View(model);
            }

            // Kiểm tra xem email đã tồn tại chưa
            if (_context.Users.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError(string.Empty, "Email is already registered.");
                return View(model);
            }

            // Gán vai trò mặc định là "User"
            model.Role = "User";

            // Mã hóa mật khẩu trước khi lưu
            model.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);

            // Thêm người dùng vào cơ sở dữ liệu
            var user = new User
            {
                UserName = model.Name,
                Email = model.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Role = "User", 
                Address = null,
                Phone = null,
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Đăng nhập ngay sau khi đăng ký thành công
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.Name ?? string.Empty),
                new Claim(ClaimTypes.Email, model.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, model.Role ?? "User")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties { IsPersistent = true };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties
            );

            // Điều hướng về trang chủ sau khi đăng ký thành công
            return RedirectToAction("Index", "Home");
        }

        // Trả về view với thông báo lỗi
        return View(model);
    }


    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }
}
