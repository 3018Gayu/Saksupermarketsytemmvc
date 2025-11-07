using Microsoft.AspNetCore.Mvc;
using Saksupermarketsytemmvc.web.Models;

namespace Saksupermarketsytemmvc.web.Controllers
{
    public class ProductController : Controller
    {
        private readonly SaksoftSupermarketSystemContext _context;

        public ProductController(SaksoftSupermarketSystemContext context)
        {
            _context = context;
        }

        // GET: Products
        public IActionResult Index()
        {
            var products = _context.Products.ToList();
            return View(products);
        }
    }
}
