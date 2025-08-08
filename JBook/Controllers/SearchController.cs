using JBook.Models;
using JBook.Shared.Models;
using JBookCrawler.Factory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace JBook.Controllers
{
    public class SearchController : Controller
    {
        private readonly CrawlerFactory _factory;
        private readonly IMemoryCache _cache;
        private readonly BookContext _db;

        public SearchController(
            CrawlerFactory factory,
            IMemoryCache cache,
            BookContext db)
        {
            _factory = factory;
            _cache = cache;
            _db = db;
        }

        // GET: /Search/Result?keyword=...&format=...
        public async Task<IActionResult> Result(string keyword, string? format = null)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return RedirectToAction("Index", "Home");

            // Query the database for user shares matching keywords (without filtering the format first)
            var allSharedRaw = _db.BookLinks
                .Where(bl =>
                    EF.Functions.Like(bl.Title, $"%{keyword}%") ||
                    EF.Functions.Like(bl.Author, $"%{keyword}%"))
                .Select(bl => new Book
                {
                    Id = bl.Id,
                    Title = bl.Title,
                    Author = bl.Author,
                    Url = bl.Url,
                    Description = bl.Description,
                    Format = bl.Format
                })
                .ToList();

            // The unified format is named Standard Five Categories, Unknown/Empty -> Other (for easy counting and filtering consistency)
            string Normalize(string? f)
            {
                if (string.IsNullOrWhiteSpace(f)) return "Other";
                var v = f.Trim().ToUpperInvariant();
                return v switch
                {
                    "TXT" => "TXT",
                    "EPUB" => "EPUB",
                    "MOBI" => "MOBI",
                    "PDF" => "PDF",
                    _ => "Other"
                };
            }

            var allShared = allSharedRaw
                .Select(b => new Book
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    Url = b.Url,
                    Description = b.Description,
                    Format = Normalize(b.Format)
                })
                .ToList();

            // Count the five formats, ensuring each key exists
            var formats = new[] { "TXT", "EPUB", "MOBI", "PDF", "Other" };
            var formatCounts = formats.ToDictionary(f => f, f => 0);
            foreach (var g in allShared.GroupBy(b => b.Format))
            {
                if (!formatCounts.ContainsKey(g.Key)) formatCounts[g.Key] = 0;
                formatCounts[g.Key] = g.Count();
            }

            // Apply format filtering: Only filter if the format count is >= 1; otherwise, prompt and do not filter
            string? appliedFormat = null;
            var userUploaded = allShared; // No filtering by default

            if (!string.IsNullOrWhiteSpace(format))
            {
                var fmt = Normalize(format);
                var cnt = formatCounts.TryGetValue(fmt, out var c) ? c : 0;

                if (cnt >= 1)
                {
                    appliedFormat = fmt;
                    userUploaded = allShared.Where(b => b.Format == fmt).ToList();
                }
                else
                {
                    // Quantity is 0: Prompt + No filter
                    TempData["FormatZero"] = $"Cannot see the source of the book without that format???（{fmt}）";
                }
            }

            // Calling crawlers (not affected by format filtering)
            var zlibCrawler = _factory.GetCrawler("zlibrary");
            var openlibCrawler = _factory.GetCrawler("openlibrary");

            var zlibResults = zlibCrawler != null ? await zlibCrawler.SearchBooksAsync(keyword) : new List<Book>();
            var openlibResults = openlibCrawler != null ? await openlibCrawler.SearchBooksAsync(keyword) : new List<Book>();

            // Assemble the model
            var model = new CombinedSearchResult
            {
                UserBooks = userUploaded,
                ZLibraryBooks = zlibResults,
                OpenLibraryBooks = openlibResults
            };

            // Passing to the view
            ViewBag.Keyword = keyword;
            ViewBag.SelectedFormat = appliedFormat;    // Set only if the filter is actually applied
            ViewBag.FormatCounts = formatCounts;       // Always include TXT/EPUB/MOBI/PDF/Other

            return View(model);
        }
    }
}