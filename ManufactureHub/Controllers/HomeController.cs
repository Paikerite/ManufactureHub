using ManufactureHub.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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

        //[Route("404")]
        //public IActionResult PageNotFound()
        //{
        //    string originalPath = "unknown";
        //    if (HttpContext.Items.ContainsKey("originalPath"))
        //    {
        //        originalPath = HttpContext.Items["originalPath"] as string;
        //    }
        //    return View();
        //}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
