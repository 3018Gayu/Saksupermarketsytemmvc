using Microsoft.AspNetCore.Mvc;

namespace Saksupermarketsytemmvc.web.Controllers
{
    public class HomeController : Controller
    {
        // HOME PAGE
        public IActionResult Index()
        {
            return View();
        }

        // PRIVACY POLICY PAGE
        public IActionResult Privacy()
        {
            return View();
        }

        // OPTIONAL ERROR PAGE (GOOD PRACTICE)
        public IActionResult Error()
        {
            return View();
        }
    }
}
  