using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using HotelBooker.Data;
using HotelBooker.Models;
using HotelBooker.Utils.ConfigOptions.VNPay;
using HotelBooker.Services; // Thêm dòng này để sử dụng RoomService
using HotelBooker.Controllers; // Add this line to use UserController
using Microsoft.AspNetCore.Identity; // Add this line to use UserManager
using Microsoft.AspNetCore.Mvc; // Thêm dòng này để sử dụng AutoValidateAntiforgeryTokenAttribute
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // Add this line to use AddEntityFrameworkStores

var builder = WebApplication.CreateBuilder(args);

// Configure DbContext cho HotelBooker
builder.Services.AddDbContext<HotelBookerContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("HotelBookerContext") 
    ?? throw new InvalidOperationException("Connection string 'HotelBookerContext' not found.")));

// Đăng ký Identity vào container DI
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<HotelBookerContext>()
    .AddDefaultTokenProviders();

 // Đảm bảo dịch vụ được đăng ký ở đây
AddScoped();
// Add services to the container
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation(); // Hỗ trợ reload khi thay đổi Views

builder.Services.AddRazorPages()
    .AddMvcOptions(options =>
    {
        options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()); // Bảo vệ CSRF
    });

// Đăng ký dịch vụ xác thực và quyền
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/User/Login";   
        options.AccessDeniedPath = "/Home/AccessDenied";
    });

builder.Services.AddAuthorization();

// Tạo ứng dụng
var app = builder.Build();

// Tạo tài khoản Admin mặc định
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await UserController.CreateAdminUser(userManager, roleManager); // Tạo tài khoản Admin mặc định
}

// Tạo tài khoản Admin mặc định sau khi cấu hình các dịch vụ
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await UserController.CreateAdminUser(userManager, roleManager); // Tạo tài khoản Admin mặc định
}



// Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication(); // Thêm middleware xác thực
app.UseAuthorization();

// Cấu hình route
app.MapControllerRoute(
    name: "Admin",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
void AddScoped()
{
    builder.Services.AddScoped<RoomService>(); 
    builder.Services.AddScoped<HotelService>(); 
    builder.Services.AddTransient<IVNPayService, VNPayService>();
    builder.Services.Configure<VNPayConfigOptions>(builder.Configuration.GetSection("VnPay"));
}