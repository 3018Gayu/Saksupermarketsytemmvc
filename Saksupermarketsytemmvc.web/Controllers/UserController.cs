using Microsoft.AspNetCore.Mvc;

namespace Saksupermarketsytemmvc.web.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
