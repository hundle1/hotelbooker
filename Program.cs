using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using HotelBooker.Data;
using HotelBooker.Services; // Đảm bảo import đúng namespace

var builder = WebApplication.CreateBuilder(args);

// Configure DbContext for SQLite
builder.Services.AddDbContext<HotelBookerContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("HotelBookerContext") 
                      ?? throw new InvalidOperationException("Connection string 'HotelBookerContext' not found.")));

// Add services to the container
builder.Services.AddControllersWithViews();

// Register RoomService
builder.Services.AddScoped<RoomService>();
builder.Services.AddScoped<HotelService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // Configure HSTS for production
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "Admin",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// Map default controller route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
