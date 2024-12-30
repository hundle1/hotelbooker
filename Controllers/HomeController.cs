using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HotelBooker.Models;
using HotelBooker.Services;

namespace HotelBooker.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly RoomService _roomService;

    public HomeController(ILogger<HomeController> logger, RoomService roomService)
    {
        _logger = logger;
        _roomService = roomService;
    }

    public async Task<IActionResult> Index()
        {
            var rooms = await _roomService.GetAllRoomsAsync(); 
            return View(rooms);
        }

    public IActionResult Privacy()
    {
        return View();
    }
    public IActionResult Blog()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
