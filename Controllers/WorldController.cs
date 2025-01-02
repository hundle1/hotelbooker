using HotelBooker.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooker.Controllers
{
    public class WolrdController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
