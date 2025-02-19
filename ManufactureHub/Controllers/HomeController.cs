using Microsoft.AspNetCore.Mvc;

namespace ManufactureHub.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {
        }

        // GET: Privacy
        public IActionResult Privacy()
        {
            return View();
        }
    }
}
