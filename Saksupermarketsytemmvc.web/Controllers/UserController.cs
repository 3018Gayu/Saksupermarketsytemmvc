using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Saksupermarketsytemmvc.web.Models;
using System.Threading.Tasks;

namespace Saksupermarketsytemmvc.web.Controllers
{
    public class UserController : Controller
    {
        private readonly SaksoftSupermarketSystemContext _context;

        public UserController(SaksoftSupermarketSystemContext context)
        {
            _context = context;
        }

        // GET: User/Index
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users.ToListAsync();
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.ErrorMessage = TempData["ErrorMessage"];
            return View(users);
        }

        // GET: User/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null) return NotFound();
            return View(user);
        }

        // GET: User/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "User created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", $"Database error: {ex.InnerException?.Message ?? ex.Message}");
                }
            }
            return View(user);
        }

        // GET: User/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            return View(user);
        }

        // POST: User/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User user)
        {
            if (id != user.UserId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == id);
                    if (existingUser == null) return NotFound();

                    // Keep old password if left blank
                    if (string.IsNullOrEmpty(user.PasswordHash))
                    {
                        user.PasswordHash = existingUser.PasswordHash;
                    }

                    _context.Update(user);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "User updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", $"Database error: {ex.InnerException?.Message ?? ex.Message}");
                }
            }
            return View(user);
        }

        // GET: User/Delete/{id}
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null) return NotFound();

            ViewBag.ErrorMessage = TempData["ErrorMessage"];
            return View(user);
        }

        // POST: User/DeleteConfirmed
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                try
                {
                    _context.Users.Remove(user);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "User deleted successfully!";
                }
                catch (DbUpdateException)
                {
                    TempData["ErrorMessage"] = "Cannot delete this user because it may be referenced elsewhere.";
                    return RedirectToAction(nameof(Delete), new { id });
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
