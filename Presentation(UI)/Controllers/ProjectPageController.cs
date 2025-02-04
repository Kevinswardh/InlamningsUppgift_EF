using Microsoft.AspNetCore.Mvc;

namespace Presentation_UI_.Controllers
{
    public class ProjectPageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
