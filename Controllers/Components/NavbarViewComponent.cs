using Microsoft.AspNetCore.Mvc;

namespace HotelBooker.Controllers.Components
{
    public class NavbarViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View("Default");
        }
    }
}