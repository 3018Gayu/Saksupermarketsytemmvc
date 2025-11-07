using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Saksupermarketsytemmvc.web.Models;

namespace Saksupermarketsytemmvc.web.Controllers
{
    public class OrderDetailController : Controller
    {
        private readonly SaksoftSupermarketSystemContext _context;
        public OrderDetailController(SaksoftSupermarketSystemContext context)
        {
            _context = context;
        }

        // GET: OrderDetail
        public async Task<IActionResult> Index()
        {
            // Ensure we pass an actual list to the view. If the DbSet is null, return empty list.
            var list = _context?.OrderDetails != null
                ? await _context.OrderDetails.ToListAsync()
                : new List<OrderDetail>();
            return View(list);
        }
    }
}
