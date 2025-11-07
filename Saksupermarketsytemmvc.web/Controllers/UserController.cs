using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Saksupermarketsytemmvc.web.Models;

namespace Saksupermarketsytemmvc.web.Controllers
{
    public class UserController : Controller
    {
        private readonly SaksoftSupermarketSystemContext _context;
        public UserController(SaksoftSupermarketSystemContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            // Ensure we pass an actual list to the view. If the DbSet is null, return empty list.
            var list = _context?.Users != null
                ? await _context.Users.ToListAsync()
                : new List<User>();
            return View(list);
        }
    }
}
