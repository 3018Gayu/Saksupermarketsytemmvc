using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Saksupermarketsytemmvc.web.Models;
using Saksupermarketsytemmvc.web.Models.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

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

        [HttpGet]
        public IActionResult Login()
        {
            // ✅ Check if JWT cookie exists
            if (HttpContext.Request.Cookies.TryGetValue("jwtToken", out var jwtToken))
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);

                try
                {
                    // Validate JWT token
                    tokenHandler.ValidateToken(jwtToken, new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = true,
                        ValidIssuer = _config["Jwt:Issuer"],
                        ValidateAudience = true,
                        ValidAudience = _config["Jwt:Audience"],
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    }, out SecurityToken validatedToken);

                    var jwt = (JwtSecurityToken)validatedToken;
                    var userName = jwt.Claims.First(c => c.Type == ClaimTypes.Name).Value;
                    var userRole = jwt.Claims.First(c => c.Type == ClaimTypes.Role).Value;

                    TempData["UserName"] = userName;
                    TempData["UserRole"] = userRole;

                    // Redirect to Welcome page
                    return RedirectToAction("Welcome");
                }
                catch
                {
                    // Invalid or expired token → delete cookie and show login
                    HttpContext.Response.Cookies.Delete("jwtToken");
                }
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.UserEmail == model.Email && u.Isactive == "Active");

            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                ModelState.AddModelError("", "Invalid credentials or inactive account.");
                return View(model);
            }

            // Generate JWT
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
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            HttpContext.Response.Cookies.Append("jwtToken", tokenString, new CookieOptions
            {
                HttpOnly = true,
                Secure = HttpContext.Request.IsHttps,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(Convert.ToDouble(_config["Jwt:ExpireHours"] ?? "1"))
            });

            TempData["UserName"] = user.UserName;
            TempData["UserRole"] = user.UserRole;

            // Redirect to Welcome page
            return RedirectToAction("Welcome");
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

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

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Response.Cookies.Delete("jwtToken");
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Welcome()
        {
            if (!TempData.ContainsKey("UserName"))
            {
                // No user info → redirect to login
                return RedirectToAction("Login");
            }

            ViewBag.UserName = TempData["UserName"];
            ViewBag.UserRole = TempData["UserRole"];

            return View();
        }
    }
}
