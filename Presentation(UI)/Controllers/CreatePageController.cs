using Microsoft.AspNetCore.Mvc;

namespace Presentation_UI_.Controllers
{
    public class CreatePageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
