using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelBooker.Data;
using HotelBooker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace HotelBooker.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserManageController : Controller
    {
        private readonly HotelBookerContext _context;

        public UserManageController(HotelBookerContext context)
        {
            _context = context;
        }

        // GET: Admin/UserManage
        public async Task<IActionResult> Index(string searchString, string sortOrder)
        {
            // Lấy danh sách người dùng từ cơ sở dữ liệu
            var users = from u in _context.Users
                        select u;

            // Tìm kiếm theo tên người dùng hoặc email
            if (!string.IsNullOrEmpty(searchString))
            {
                users = users.Where(u => (u.UserName != null && u.UserName.Contains(searchString)) || (u.Email != null && u.Email.Contains(searchString)));
            }

            // Sắp xếp theo tên hoặc số điện thoại
            switch (sortOrder)
            {
                case "name-asc":
                    users = users.OrderBy(u => u.UserName);
                    break;
                case "name-desc":
                    users = users.OrderByDescending(u => u.UserName);
                    break;
                case "phone-asc":
                    users = users.OrderBy(u => u.PhoneNumber);
                    break;
                case "phone-desc":
                    users = users.OrderByDescending(u => u.PhoneNumber);
                    break;
                default:
                    users = users.OrderBy(u => u.UserName);
                    break;
            }

            // Lấy vai trò của tất cả người dùng
            var userManager = _context.GetService<UserManager<User>>();
            var userRoles = new Dictionary<string, List<string>>();
            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);
                userRoles[user.Id] = roles.ToList();
            }
            ViewBag.UserRoles = userRoles;

            return View(await users.ToListAsync());
        }



        // GET: Admin/UserManage/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            // Lấy thông tin user
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Lấy danh sách role của user
            var userManager = _context.GetService<UserManager<User>>();
            var roles = await userManager.GetRolesAsync(user);
            ViewBag.Roles = roles.ToList();

            return View(user);
        }


        // Thêm phần chỉnh sửa chỉ Role
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            // Lấy danh sách các roles
            var userManager = _context.GetService<UserManager<User>>();
            ViewBag.Roles = new List<string> { "Admin", "User", "BadGirl","Staff" }; // Thay đổi danh sách role tùy theo ứng dụng của bạn.
            ViewBag.UserRoles = await userManager.GetRolesAsync(user); // Lấy vai trò của người dùng hiện tại.
            return View(user);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, string role)
        {
            if (id == null || string.IsNullOrWhiteSpace(role)) return NotFound();

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            try
            {
                // Gán role mới cho user
                var userRoles = await _context.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == user.Id);
                if (userRoles != null)
                {
                    _context.UserRoles.Remove(userRoles);
                    await _context.SaveChangesAsync();
                }

                var newRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == role);
                if (newRole != null)
                {
                    _context.UserRoles.Add(new IdentityUserRole<string> { UserId = user.Id, RoleId = newRole.Id });
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/UserManage/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();

            return View(user);
        }

        // POST: Admin/UserManage/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
