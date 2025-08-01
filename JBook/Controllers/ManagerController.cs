using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JBook.Models;
using Microsoft.EntityFrameworkCore;

namespace JBook.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ManagerController : Controller
    {
        private readonly BookContext _db;

        public ManagerController(BookContext db)
        {
            _db = db;
        }

        // GET: /Manager/Manager
        public async Task<IActionResult> Manager()
        {
            // 读取所有 Report
            var allReports = await _db.Reports
                                      .OrderByDescending(r => r.ReportedAt)
                                      .ToListAsync();
            return View(allReports);
        }

        // POST: /Manager/DeleteBookLink/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBookLink(int linkId)
        {
            // 找到对应的 BookLink
            var link = await _db.BookLinks.FindAsync(linkId);
            if (link != null)
            {
                _db.BookLinks.Remove(link);

                // 可选：同时把与之关联的 Report 一并删除
                var relatedReports = _db.Reports.Where(r => r.Id == linkId);
                _db.Reports.RemoveRange(relatedReports);

                await _db.SaveChangesAsync();
                TempData["ManagerMsg"] = $"BookLink #{linkId} and related reports deleted.";
            }
            else
            {
                TempData["ManagerMsg"] = $"BookLink #{linkId} not found.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}