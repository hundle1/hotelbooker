using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using HotelBooker.Models;
using System.Threading.Tasks;
using HotelBooker.ViewModels;
using HotelBooker.Services;

namespace HotelBooker.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly HotelService _hotelService;

        public UserController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager, HotelService hotelService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _hotelService = hotelService;
        }

        // GET: /User/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        // POST: /User/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(model.UserName))
                {
                    ModelState.AddModelError(string.Empty, "Username cannot be null or empty.");
                }
                else
                {
                    if (string.IsNullOrEmpty(model.Password))
                    {
                        ModelState.AddModelError(string.Empty, "Password cannot be null or empty.");
                    }
                    else
                    {
                        var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
                        if (result.Succeeded)
                        {
                            return RedirectToAction("Index", "Home");
                        }
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    }
                }
            }
            return View(model);
        }

        // GET: /User/SignUp
        [HttpGet]
        public IActionResult SignUp()
        {
            return View(new SignUpViewModel());
        }

        // POST: /User/SignUp
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            if (ModelState.IsValid)
            {
                var roleExists = await _roleManager.RoleExistsAsync("User");
                if (!roleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole("User"));
            }

            // Tạo tài khoản mới
            var user = new User { UserName = model.UserName, Email = model.Email, Status = UserStatus.Active };
            if (string.IsNullOrEmpty(model.Password))
            {
                ModelState.AddModelError(string.Empty, "Password cannot be null or empty.");
                return View(model);
            }

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                // Gán vai trò "User" cho tài khoản mới
                var roleResult = await _userManager.AddToRoleAsync(user, "User");
                if (roleResult.Succeeded)
                {
                    // Đăng nhập tự động nếu tạo và gán vai trò thành công
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // Xóa tài khoản nếu gán vai trò thất bại
                    await _userManager.DeleteAsync(user);
                    ModelState.AddModelError(string.Empty, "Failed to assign role. User creation reverted.");
                }
                }
                else
                {
                    // Hiển thị lỗi nếu tạo tài khoản thất bại
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            // Thông báo thất bại nếu có lỗi
            ViewData["SignUpSuccess"] = "Tạo tài khoản thất bại. Vui lòng thử lại!";
            foreach (var key in ModelState.Keys)
            {
                if (ModelState[key] != null)
                {
                    var errors = ModelState[key]?.Errors;
                    if (errors != null)
                    {
                        foreach (var error in errors)
                        {
                            Console.WriteLine($"Key: {key}, Error: {error.ErrorMessage}");
                        }
                    }
                }
            }
            return View(model);
        }


        // POST: /User/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();  // Đảm bảo người dùng được đăng xuất
            return RedirectToAction("Index", "Home");  // Sau khi đăng xuất, chuyển hướng về trang chủ
        }

        // Tạo tài khoản Admin mặc định
        public static async Task CreateAdminUser(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Kiểm tra xem vai trò Admin có tồn tại chưa, nếu chưa thì tạo
            var roleExist = await roleManager.RoleExistsAsync("Admin");
            if (!roleExist)
            {
                // Tạo vai trò "Admin" nếu chưa tồn tại
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // Kiểm tra xem tài khoản Admin đã tồn tại chưa
            var adminUser = await userManager.FindByNameAsync("Admin");
            if (adminUser == null)
            {
                // Tạo tài khoản Admin mới
                var user = new User { UserName = "Admin", Email = "admin@example.com", Status = UserStatus.Active };
                var createResult = await userManager.CreateAsync(user, "Admin@123");

                if (createResult.Succeeded)
                {
                    // Thêm tài khoản Admin vào vai trò "Admin"
                    var addToRoleResult = await userManager.AddToRoleAsync(user, "Admin");

                    if (addToRoleResult.Succeeded)
                    {
                        Console.WriteLine("Admin role assigned successfully.");
                    }
                    else
                    {
                        // Nếu thêm vai trò thất bại, in ra lỗi
                        foreach (var error in addToRoleResult.Errors)
                        {
                            Console.WriteLine($"Error adding user to role: {error.Description}");
                        }
                    }
                }
                else
                {
                    // In ra các lỗi nếu có
                    foreach (var error in createResult.Errors)
                    {
                        Console.WriteLine($"Error creating user: {error.Description}");
                    }
                }
            }
            else
            {
                // Nếu tài khoản Admin đã tồn tại, gán lại vai trò "Admin"
                var isInRole = await userManager.IsInRoleAsync(adminUser, "Admin");
                if (!isInRole)
                {
                    var addToRoleResult = await userManager.AddToRoleAsync(adminUser, "Admin");

                    if (addToRoleResult.Succeeded)
                    {
                        Console.WriteLine("Admin role assigned successfully.");
                    }
                    else
                    {
                        foreach (var error in addToRoleResult.Errors)
                        {
                            Console.WriteLine($"Error adding user to role: {error.Description}");
                        }
                    }
                }
            }
        }
        [HttpGet]
        public async Task<IActionResult> UserInfor()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }
            var orders = await _hotelService.GetOrdersByUserIdAsync(user.Id);  // Assuming this method exists to fetch orders
            ViewData["Orders"] = orders;
            return View(user);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(User model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }
            // Cập nhật thông tin không bao gồm mật khẩu và vai trò
            user.ImageURL = model.ImageURL;
            user.Address = model.Address;
            user.Birth = model.Birth;
            user.UserName = model.UserName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("UserInfor");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }
    }
}
