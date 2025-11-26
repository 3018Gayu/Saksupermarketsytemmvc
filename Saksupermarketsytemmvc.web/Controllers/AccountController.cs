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

        // ---------------- LOGIN ----------------
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserEmail == model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "No user found with this email.");
                return View(model);
            }

            if (user.Isactive != "Active")
            {
                ModelState.AddModelError("", "Your account is inactive.");
                return View(model);
            }

            if (!BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                ModelState.AddModelError("", "Incorrect password.");
                return View(model);
            }

            // ----------- JWT GENERATION -------------
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);

            double expireMinutes = Convert.ToDouble(_config["Jwt:ExpireMinutes"] ?? "5");

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.UserEmail),
                new Claim(ClaimTypes.Role, user.UserRole)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expireMinutes),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            Response.Cookies.Append("jwtToken", tokenString, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(expireMinutes)
            });

            return RedirectToAction("Welcome");
        }

        // ---------------- REGISTER ----------------
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.UserEmail == model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("", "Email already registered.");
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

            return RedirectToAction("Login");
        }

        // ---------------- FORGOT PASSWORD ----------------
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserEmail == model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "No user found with this email.");
                return View(model);
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login");
        }

        // ---------------- LOGOUT ----------------
        [HttpGet]
        [Authorize]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwtToken");
            return RedirectToAction("LoggingOff");
        }

        // ---------------- LOGGING OFF PAGE ----------------
        [HttpGet]
        [AllowAnonymous]
        public IActionResult LoggingOff()
        {
            return View();
        }

        // ---------------- WELCOME ----------------
        [HttpGet]
        [Authorize]
        public IActionResult Welcome()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("LoggingOff");

            ViewBag.UserName = User.Identity.Name;
            ViewBag.UserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            return View();
        }
    }
}
