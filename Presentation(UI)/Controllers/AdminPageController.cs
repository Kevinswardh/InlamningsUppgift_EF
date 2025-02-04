using Microsoft.AspNetCore.Mvc;

namespace Presentation_UI_.Controllers
{
    public class AdminPageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
