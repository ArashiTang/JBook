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
            // Display the share form directly (the front-end JS has intercepted the non-logged-in click)
            ViewBag.SuccessMessage = TempData["ShareSuccess"] as string;
            return View();
        }

        // POST: /Share/ShareLink
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ShareLink(BookLink model)
        {
            // Secondary protection: return 401 if not logged in
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();

            if (!ModelState.IsValid)
            {
                // Verification failed, retain form error
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