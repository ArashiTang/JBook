using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using JBook.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace JBook.Controllers
{
    public class AccountController : Controller
    {
        private readonly BookContext _db;

        public AccountController(BookContext db)
        {
            _db = db;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string Email, string Password)
        {
            // 尝试普通用户表
            var user = _db.Users
                          .FirstOrDefault(u => u.Email == Email && u.Password == Password);

            string role;
            string name;

            if (user != null)
            {
                role = "User";
                name = user.Nickname;
            }
            else
            {
                // 尝试管理员表
                var admin = _db.Admins
                               .FirstOrDefault(a => a.Email == Email && a.Password == Password);

                if (admin == null)
                {
                    TempData["LoginError"] = "Email or password is incorrect.";
                    return RedirectToAction("Index", "Home");
                }

                role = "Admin";
                name = admin.Nickname;
            }

            // 构造 Claims 并写入 Cookie
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Role, role)
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            string Nickname,
            string Email,
            string Password,
            string ConfirmPassword)
        {
            // 简单校验
            if (Password != ConfirmPassword)
            {
                TempData["RegisterError"] = "Passwords do not match.";
                return RedirectToAction("Index", "Home");
            }
            if (_db.Users.Any(u => u.Email == Email))
            {
                TempData["RegisterError"] = "Email already registered.";
                return RedirectToAction("Index", "Home");
            }

            // 创建并保存新用户
            var user = new User
            {
                Nickname = Nickname,
                Email = Email,
                Password = Password
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            // 自动登录
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Nickname),
                new Claim(ClaimTypes.Role, "User")
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}