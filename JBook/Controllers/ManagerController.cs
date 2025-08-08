// Controllers/ManagerController.cs
using JBook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JBook.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ManagerController : Controller
    {
        private readonly BookContext _db;
        public ManagerController(BookContext db) => _db = db;

        // GET: /Manager/Manager
        public async Task<IActionResult> Manager()
        {
            // Read out all reports
            var reports = await _db.Reports
                                   .OrderByDescending(r => r.ReportedAt)
                                   .ToListAsync();
            return View(reports);
        }

        // POST: /Manager/Execute/123
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Execute(int reportId)
        {
            var rpt = await _db.Reports.FindAsync(reportId);
            if (rpt != null)
            {
                // 1. Delete the corresponding BookLink (if it exists)
                var link = await _db.BookLinks.FindAsync(rpt.LinkId);
                if (link != null)
                    _db.BookLinks.Remove(link);

                // 2. Delete all reports for this LinkId
                var related = _db.Reports.Where(r => r.LinkId == rpt.LinkId);
                _db.Reports.RemoveRange(related);

                await _db.SaveChangesAsync();
                TempData["ManagerMsg"] = $"Executed: removed Link#{rpt.LinkId} and its reports.";
            }
            return RedirectToAction(nameof(Manager));
        }

        // POST: /Manager/Ignore/123
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ignore(int reportId)
        {
            var rpt = await _db.Reports.FindAsync(reportId);
            if (rpt != null)
            {
                _db.Reports.Remove(rpt);
                await _db.SaveChangesAsync();
                TempData["ManagerMsg"] = $"Ignored report #{reportId}.";
            }
            return RedirectToAction(nameof(Manager));
        }
    }
}