//Tang Jiongzheng(c3509120)//
using System.Diagnostics;
using JBook.Models;
using Microsoft.AspNetCore.Mvc;

namespace JBook.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult User()
        {
            return View(); // 后续创建 Views/Home/User.cshtml
        }
    }
}

