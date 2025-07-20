using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<IActionResult> Result(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return RedirectToAction("Index", "Home");

            // Construction Cache
            string cacheKey = $"search_{keyword}";

            // Cache Read
            if (!_cache.TryGetValue(cacheKey, out CombinedSearchResult model))
            {
                // Cache miss: first check the BookLink shared by the user from the database
                var userUploaded = _db.BookLinks
                    .Where(bl =>
                        EF.Functions.Like(bl.Title, $"%{keyword}%") ||
                        EF.Functions.Like(bl.Author, $"%{keyword}%"))
                    .Select(bl => new Book
                    {
                        Title = bl.Title,
                        Author = bl.Author,
                        Url = bl.Url,
                        Format = bl.Format,
                        Description = bl.Description
                    })
                    .ToList();

                // Two crawler sources
                var zlibCrawler = _factory.GetCrawler("zlibrary");
                var openlibCrawler = _factory.GetCrawler("openlibrary");

                var zlibResults = zlibCrawler != null
                    ? await zlibCrawler.SearchBooksAsync(keyword)
                    : new List<Book>();

                var openlibResults = openlibCrawler != null
                    ? await openlibCrawler.SearchBooksAsync(keyword)
                    : new List<Book>();

                // Result
                model = new CombinedSearchResult
                {
                    UserBooks = userUploaded,
                    ZLibraryBooks = zlibResults,
                    OpenLibraryBooks = openlibResults
                };

                // Write cache, 30 minutes expiration
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                };
                _cache.Set(cacheKey, model, cacheOptions);
            }

            ViewBag.Keyword = keyword;

            // Return to view
            return View(model);
        }
    }
}