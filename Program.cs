using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using HotelBooker.Data;
using HotelBooker.Models;
using HotelBooker.Services; // Thêm dòng này để sử dụng RoomService

var builder = WebApplication.CreateBuilder(args);

// Configure DbContext cho HotelBooker
builder.Services.AddDbContext<HotelBookerContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("HotelBookerContext") ?? throw new InvalidOperationException("Connection string 'HotelBookerContext' not found.")));

// Đăng ký dịch vụ RoomService vào container DI
builder.Services.AddScoped<RoomService>();  // Thêm dòng này
builder.Services.AddScoped<HotelService>();  // Đảm bảo dịch vụ được đăng ký ở đâ

// Add services to the container
builder.Services.AddControllersWithViews();

// Đăng ký dịch vụ xác thực
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";   
        options.AccessDeniedPath = "/Home/AccessDenied";
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Tạo admin mặc định
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<HotelBookerContext>(); 
    db.Database.EnsureCreated();

    if (!db.Users.Any())
    {
        db.Users.Add(new User
        {
            UserName = "trexbairong",
            Email = "trexbairong@gmail.com",
            Password = BCrypt.Net.BCrypt.HashPassword("123456"),
            Role = "Admin",
            Status = UserStatus.Active
        });
        db.SaveChanges();
    }
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

app.MapControllerRoute(
    name: "Admin",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
