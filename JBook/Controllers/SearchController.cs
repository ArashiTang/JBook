// Tang Jiongzheng (c3509120) //
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JBook.Shared.Models;
using JBookCrawler.Factory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace JBook.Controllers
{
    public class SearchController : Controller
    {
        private readonly CrawlerFactory _factory;
        private readonly IMemoryCache _cache;

        public SearchController(CrawlerFactory factory, IMemoryCache cache)
        {
            _factory = factory;
            _cache = cache;
        }

        public async Task<IActionResult> Result(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return RedirectToAction("Index", "Home");

            // Cache key
            string cacheKey = $"search_{keyword}";
            if (!_cache.TryGetValue(cacheKey, out CombinedSearchResult model))
            {
                // Cache miss: call crawler
                var zlibCrawler = _factory.GetCrawler("zlibrary");
                var openLibCrawler = _factory.GetCrawler("openlibrary");

                var zlibResults = zlibCrawler != null ? await zlibCrawler.SearchBooksAsync(keyword) : new List<Book>();
                var openLibResults = openLibCrawler != null ? await openLibCrawler.SearchBooksAsync(keyword) : new List<Book>();

                // User upload example (subsequent database replacement)
                var userUploaded = new List<Book>
                {
                    new Book { Title="Sample Books", Author="UserA", Url="https://example.com", Description="PDF" }
                };

                // Construct CombinedSearchResult
                model = new CombinedSearchResult
                {
                    UserBooks = userUploaded,
                    ZLibraryBooks = zlibResults,
                    OpenLibraryBooks = openLibResults
                };

                // Store in cache (expires in 30 minutes)
                var options = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                };
                _cache.Set(cacheKey, model, options);
            }

            // Return view (view @model is CombinedSearchResult)
            return View(model);
        }
    }
}