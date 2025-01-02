using Microsoft.AspNetCore.Mvc;

namespace HotelBooker.Controllers.Components
{
    public class NavbarViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("Default");
        }
    }
}