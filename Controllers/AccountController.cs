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
        if (user != null && password == user.Password)
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
    public async Task<IActionResult> SignUp(User model, string confirmPassword)
    {
        if (ModelState.IsValid)
        {
            if (model.Password != confirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Passwords do not match.");
                return View(model);
            }
            if (_context.Users.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError(string.Empty, "Email is already registered.");
                return View(model);
            }
            model.Role = "User";
            model.Password = model.Password;
            var user = new User
            {
                UserName = model.UserName,
                Email = model.Email,
                Password = model.Password,
                Role = "User",
                Birth = null,
                Address = null,
                Phone = null,
                Image = null,
                Gender = null
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.UserName ?? string.Empty, ClaimValueTypes.String),
                new Claim(ClaimTypes.Email, model.Email ?? string.Empty, ClaimValueTypes.String),
                new Claim(ClaimTypes.Role, model.Role ?? "User", ClaimValueTypes.String)
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
        return View(model);
    }


    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }


    public async Task<IActionResult> UserInfor()
    {
        var userName = User.Identity != null ? User.Identity.Name : null;
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
        return View(user);
    }
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        var userName = User.Identity != null ? User.Identity.Name : null;
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);

        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    // Xử lý cập nhật thông tin người dùng
    [Authorize]
    [HttpPost]
    [ActionName("Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(User model, IFormFile? Image)
    {
        if (ModelState.IsValid)
        {
            var userName = User.Identity?.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);

            if (user == null)
            {
                return NotFound();
            }

            // Cập nhật thông tin người dùng
            user.UserName = model.UserName;
            user.Email = model.Email;
            user.Phone = model.Phone;
            user.Address = model.Address;
            user.Gender = model.Gender;

            // Xử lý cập nhật mật khẩu
            if (!string.IsNullOrEmpty(model.Password))
            {
                user.Password = model.Password;
            }

            // Xử lý ảnh đại diện
            if (Image != null && Image.Length > 0)
            {
                // Đường dẫn lưu ảnh
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img");
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Image.FileName);
                var filePath = Path.Combine(uploads, fileName);

                // Lưu ảnh vào server
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await Image.CopyToAsync(fileStream);
                }

                // Cập nhật đường dẫn ảnh vào database
                user.Image = "/img/" + fileName;
            }

            // Cập nhật dữ liệu người dùng trong cơ sở dữ liệu
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("UserInfor"); // Quay lại trang thông tin người dùng
        }

        return View(model);
    }




}