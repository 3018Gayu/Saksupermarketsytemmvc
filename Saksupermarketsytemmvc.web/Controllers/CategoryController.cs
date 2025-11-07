using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Saksupermarketsytemmvc.web.Models;

namespace Saksupermarketsytemmvc.web.Controllers
{
    public class CategoryController : Controller
    {

        private readonly SaksoftSupermarketSystemContext _context;
        public CategoryController(SaksoftSupermarketSystemContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            // Ensure we pass an actual list to the view. If the DbSet is null, return empty list.
            var list = _context?.Categories != null
                ? await _context.Categories.ToListAsync()
                : new List<Category>();
            return View(list);
        }
    }
}
