using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using JBook.Models;

namespace JBook.Controllers
{
    public class ShareController : Controller
    {
        private readonly BookContext _db;

        public ShareController(BookContext db)
        {
            _db = db;
        }

        // GET: /Share/ShareLink
        [HttpGet]
        public IActionResult ShareLink()
        {
            // 直接显示分享表单（前端 JS 已拦截未登录点击）
            ViewBag.SuccessMessage = TempData["ShareSuccess"] as string;
            return View();
        }

        // POST: /Share/ShareLink
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ShareLink(BookLink model)
        {
            // 二次保护：未登录直接返回 401
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();

            if (!ModelState.IsValid)
            {
                // 校验失败，保留表单错误
                ViewBag.SuccessMessage = null;
                return View(model);
            }

            _db.BookLinks.Add(model);
            await _db.SaveChangesAsync();

            TempData["ShareSuccess"] = "Thank you for sharing";
            return RedirectToAction(nameof(ShareLink));
        }
    }
}