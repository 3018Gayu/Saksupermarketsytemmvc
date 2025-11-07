using Microsoft.AspNetCore.Mvc;
using Saksupermarketsytemmvc.web.Models;

namespace Saksupermarketsytemmvc.web.Controllers
{
    public class OrderController : Controller
    {
        private readonly SaksoftSupermarketSystemContext _context;
        public OrderController(SaksoftSupermarketSystemContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var orders = _context.Orders.ToList();
            return View(orders);
        }
    }
}
