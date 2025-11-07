using Microsoft.AspNetCore.Mvc;

namespace Saksupermarketsytemmvc.web.Controllers
{
    public class SupplierController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
