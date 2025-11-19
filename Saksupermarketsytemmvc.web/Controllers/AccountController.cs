using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Saksupermarketsytemmvc.web.Models;
using Saksupermarketsytemmvc.web.Models.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Saksupermarketsytemmvc.web.Controllers
{
    public class AccountController : Controller
    {
        private readonly SaksoftSupermarketSystemContext _context;
        private readonly IConfiguration _config;

        public AccountController(SaksoftSupermarketSystemContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // --------------------- LOGIN -----------------------------
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Check if user with this email exists
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserEmail == model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "No user found with this email.");
                return View(model);
            }

            // Check if user is active
            if (user.Isactive != "Active")
            {
                ModelState.AddModelError("", "Your account is inactive. Please contact admin.");
                return View(model);
            }

            // Check password
            if (!BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                ModelState.AddModelError("", "Incorrect password.");
                return View(model);
            }

            // ----------- GENERATE JWT TOKEN ----------------
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.UserEmail),
                new Claim(ClaimTypes.Role, user.UserRole)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(Convert.ToDouble(_config["Jwt:ExpireHours"] ?? "1")),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // Save JWT in cookie
            HttpContext.Response.Cookies.Append("jwtToken", tokenString, new CookieOptions
            {
                HttpOnly = true,
                Secure = HttpContext.Request.IsHttps,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(Convert.ToDouble(_config["Jwt:ExpireHours"] ?? "1"))
            });

            return RedirectToAction("Welcome");
        }

        // --------------------- REGISTER -----------------------------
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (await _context.Users.AnyAsync(u => u.UserEmail == model.Email))
            {
                ModelState.AddModelError("", "Email already exists.");
                return View(model);
            }

            var user = new User
            {
                UserName = model.Name,
                UserEmail = model.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                UserRole = model.Role,
                Isactive = "Active"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Registration successful! Please log in.";
            return RedirectToAction("Login");
        }

        // --------------------- FORGOT PASSWORD -----------------------------
        [HttpGet]
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserEmail == model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "Email not found.");
                return View(model);
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Password reset successful!";
            return RedirectToAction("Login");
        }

        // --------------------- LOGOUT -----------------------------
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Response.Cookies.Delete("jwtToken");
            return RedirectToAction("Login");
        }

        // --------------------- WELCOME PAGE -----------------------------
        [HttpGet]
        [Authorize]
        public IActionResult Welcome()
        {
            var userName = User.Identity?.Name ?? "Unknown";
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "Unknown";

            ViewBag.UserName = userName;
            ViewBag.UserRole = userRole;

            return View();
        }
    }
}
