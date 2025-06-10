// Tang Jiongzheng (c3509120) //
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using JBook.Shared.Models;       // ← 共享模型命名空间
using JBookCrawler.Factory;      // ← 爬虫工厂命名空间
using Microsoft.AspNetCore.Mvc;

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

            // 1. 缓存键
            string cacheKey = $"search_{keyword}";
            if (!_cache.TryGetValue(cacheKey, out CombinedSearchResult model))
            {
                // 2. 缓存未命中：调用爬虫
                var zlibCrawler = _factory.GetCrawler("zlibrary");
                var openLibCrawler = _factory.GetCrawler("openlibrary");

                var zlibResults = zlibCrawler != null ? await zlibCrawler.SearchBooksAsync(keyword) : new List<Book>();
                var openLibResults = openLibCrawler != null ? await openLibCrawler.SearchBooksAsync(keyword) : new List<Book>();

                // 3. 用户上传示例（后续替换数据库）
                var userUploaded = new List<Book>
                {
                    new Book { Title="示例书籍", Author="用户A", Url="https://example.com", Description="PDF" }
                };

                // 4. 构造 CombinedSearchResult
                model = new CombinedSearchResult
                {
                    UserBooks = userUploaded,
                    ZLibraryBooks = zlibResults,
                    OpenLibraryBooks = openLibResults
                };

                // 5. 存入缓存 (30分钟过期)
                var options = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                };
                _cache.Set(cacheKey, model, options);
            }

            // 6. 返回视图 (视图 @model 是 CombinedSearchResult)
            return View(model);
        }
    }
}
