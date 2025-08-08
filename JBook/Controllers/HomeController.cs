//Tang Jiongzheng(c3509120)//
using Microsoft.AspNetCore.Mvc;

namespace JBook.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        
    }
}