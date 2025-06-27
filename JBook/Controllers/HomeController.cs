
using System.Diagnostics;
using JBook.Models;
using Microsoft.AspNetCore.Mvc;
using MyUser = JBook.Models.User;

namespace JBook.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        // 添加方法
        public IActionResult CheckLogin(string redirect)
        {
            // 检查 Session 是否已登录
            if (HttpContext.Session.GetString("username") == null)
            {
                // 未登录，跳转到登录页，并附带返回地址
                return RedirectToAction("Login", "Home", new { returnUrl = redirect });
            }

            // 已登录，跳转到指定页面
            return RedirectToAction(redirect);
        }
        [HttpGet]
        public IActionResult Login(string returnUrl = "Index")
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password, string returnUrl = "Index")
        {
            // 简化版身份验证（可改为数据库验证）
            if (username == "admin" && password == "123456")
            {
                HttpContext.Session.SetString("username", username);
                return RedirectToAction(returnUrl);
            }
            ViewBag.Error = "Invalid username or password.";
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }


        // 注册页面展示
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // 处理注册请求

        [HttpPost]
        public IActionResult Register(string username, string password, string confirm)
        {
            if (JBook.Models.User.Users.Any(u => u.Username == username))
            {
                ViewBag.Message = "用户名已存在，请更换";
                return View();
            }

            if (password != confirm)
            {
                ViewBag.Message = "两次密码不一致";
                return View();
            }

            JBook.Models.User.Users.Add(new JBook.Models.User { Username = username, Password = password });
            HttpContext.Session.SetString("username", username);
            return RedirectToAction("Bookshelf");
        }


        // 书架页视图展示
        public IActionResult Bookshelf()
        {
            // 只有登录用户才能访问
            if (HttpContext.Session.GetString("username") == null)
            {
                return RedirectToAction("Login", new { returnUrl = "Bookshelf" });
            }

            ViewBag.Username = HttpContext.Session.GetString("username");
            return View();
        }

        public IActionResult FamousAuthor()
        {
            return View();
        }
        public IActionResult Bookstore()
        {
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("username");
            return RedirectToAction("Index");
        }

    }



}
