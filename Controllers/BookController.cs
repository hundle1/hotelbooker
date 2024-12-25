using Microsoft.AspNetCore.Mvc;

namespace HotelBooker.Controllers
{
    public class BookController : Controller
    {
        public BookController()
        {
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}