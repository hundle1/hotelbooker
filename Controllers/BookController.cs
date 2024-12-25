using Microsoft.AspNetCore.Mvc;

namespace HotelBooker.Controllers
{
    public class BookController : Controller
    {
        public BookController()
        {
        }

        public string Index()
        {
            return "Hiển thị danh sách phòng và chức năng book phòng";
        }
    }
}