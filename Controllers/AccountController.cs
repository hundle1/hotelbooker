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
    private readonly UserDbContext _context;

    public AccountController(UserDbContext context)
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
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Role, user.Role!)
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
    public IActionResult SignUp()
    {
        return View();
    }

    // POST: /Account/SignUp
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SignUp(User user, string confirmPassword)
    {
        if (ModelState.IsValid)
        {
            // Kiểm tra nếu mật khẩu và xác nhận mật khẩu khớp
            if (user.Password != confirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Passwords do not match.");
                return View(user);
            }

            // Kiểm tra xem email đã tồn tại chưa
            if (_context.Users.Any(u => u.Email == user.Email))
            {
                ModelState.AddModelError(string.Empty, "Email is already registered.");
                return View(user);
            }

            // Mã hóa mật khẩu trước khi lưu vào cơ sở dữ liệu
            user.HashPassword();

            // Thêm người dùng vào cơ sở dữ liệu
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Đăng nhập ngay sau khi đăng ký thành công
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.Role, user.Role ?? string.Empty)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties { IsPersistent = true };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties
            );

            return RedirectToAction("Index", "Home");
        }

        return View(user);
    }

    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }
}
