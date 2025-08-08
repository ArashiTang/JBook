using JBook.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JBook.Controllers
{
    public class ReportController : Controller
    {
        private readonly BookContext _db;
        public ReportController(BookContext db) => _db = db;

        // GET /Report/Create?linkId=5&returnUrl=...
        [HttpGet]
        public async Task<IActionResult> Create(int linkId, string returnUrl)
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();

            // First get the URL of this BookLink from the database
            var link = await _db.BookLinks
                                .AsNoTracking()
                                .FirstOrDefaultAsync(bl => bl.Id == linkId);
            var url = link?.Url ?? "unknown";

            // Construct message
            var user = User.Identity.Name ?? "Anonymous";
            var now = DateTime.Now;
            var msg = $"{user} reported Link#{linkId}({url}) at {now:yyyy-MM-dd HH:mm}";


            // Save to the database
            _db.Reports.Add(new Report
            {
                LinkId = linkId,
                Message = msg,
                ReportedAt = now
            });
            await _db.SaveChangesAsync();

            TempData["ReportSuccess"] = "Thanks for reporting. Our admin will review it soon.";
            return Redirect(returnUrl ?? Url.Action("Index", "Home"));
        }
    }
}