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
        public async Task<IActionResult> Result(string keyword, string format = null)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return RedirectToAction("Index", "Home");

            // First, get all user sharing links matching the keyword from the database (regardless of format)
            var allShared = _db.BookLinks
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

            // Count the number of each format
            var formatCounts = allShared
                .GroupBy(b => b.Format)
                .ToDictionary(g => g.Key, g => g.Count());

            // If a format is passed in, only the sharing results in that format will be retained
            var userUploaded = string.IsNullOrEmpty(format)
                ? allShared
                : allShared.Where(b => b.Format == format).ToList();

            // Call the crawler to obtain external data (quantity and filtering do not affect the crawler results)
            var zlibCrawler = _factory.GetCrawler("zlibrary");
            var openlibCrawler = _factory.GetCrawler("openlibrary");

            var zlibResults = zlibCrawler != null ? await zlibCrawler.SearchBooksAsync(keyword) : new List<Book>();
            var openlibResults = openlibCrawler != null ? await openlibCrawler.SearchBooksAsync(keyword) : new List<Book>();

            // Combine the final model
            var model = new CombinedSearchResult
            {
                UserBooks = userUploaded,
                ZLibraryBooks = zlibResults,
                OpenLibraryBooks = openlibResults
            };

            // Additional data passed to the view
            ViewBag.Keyword = keyword;
            ViewBag.SelectedFormat = format;    // Currently selected format
            ViewBag.FormatCounts = formatCounts;    // Dictionary of the number of formats

            return View(model);
        }
    }
}
