using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Saksupermarketsytemmvc.web.Models;

namespace Saksupermarketsytemmvc.web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly SaksoftSupermarketSystemContext _context;

        public UserController(SaksoftSupermarketSystemContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _context.Users.ToListAsync();
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.ErrorMessage = TempData["ErrorMessage"];
            return View(users);
        }

        public async Task<IActionResult> Details(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(user.PasswordHash))
                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

                user.Isactive ??= "Active";

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "User created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User user)
        {
            if (id != user.UserId) return NotFound();

            if (ModelState.IsValid)
            {
                var existingUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == id);
                if (existingUser == null) return NotFound();

                if (string.IsNullOrEmpty(user.PasswordHash))
                    user.PasswordHash = existingUser.PasswordHash;
                else
                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

                user.Isactive ??= existingUser.Isactive;

                _context.Update(user);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "User updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null) return NotFound();
            ViewBag.ErrorMessage = TempData["ErrorMessage"];
            return View(user);
        }

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
