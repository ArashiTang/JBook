using System;
using System.Threading.Tasks;
using JBook.Models;
using Microsoft.AspNetCore.Mvc;

namespace JBook.Controllers
{
    public class ReportController : Controller
    {
        private readonly BookContext _db;

        public ReportController(BookContext db)
        {
            _db = db;
        }

        // GET /Report/Create?linkId=5&returnUrl=/Search/Result?keyword=xxx
        [HttpGet]
        public async Task<IActionResult> Create(int linkId, string returnUrl)
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();

            // 构造消息
            var user = User.Identity.Name ?? "Anonymous";
            var now = DateTime.Now;
            var msg = $"{user} reported BookLink#{linkId} at {now:yyyy-MM-dd HH:mm:ss}";

            // 存库
            _db.Reports.Add(new Report
            {
                Message = msg,
                ReportedAt = now
            });
            await _db.SaveChangesAsync();

            // 用 TempData 带个提示
            TempData["ReportSuccess"] = "Thanks for reporting. Our admin will review it soon.";

            // 重定向回原来页面
            return Redirect(returnUrl ?? Url.Action("Index", "Home"));
        }
    }
}