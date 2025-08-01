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

        // GET: /Search/Result?keyword=...&format=...
        public async Task<IActionResult> Result(string keyword, string format = null)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return RedirectToAction("Index", "Home");

            // 首先，从数据库中获取所有匹配关键字的用户分享链接（不区分格式）
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

            // 统计各格式的数量
            var formatCounts = allShared
                .GroupBy(b => b.Format)
                .ToDictionary(g => g.Key, g => g.Count());

            // 如果传入了 format，则仅保留该格式的分享结果
            var userUploaded = string.IsNullOrEmpty(format)
                ? allShared
                : allShared.Where(b => b.Format == format).ToList();

            // 调用爬虫获取外部数据（数量、过滤不影响爬虫结果）
            var zlibCrawler = _factory.GetCrawler("zlibrary");
            var openlibCrawler = _factory.GetCrawler("openlibrary");

            var zlibResults = zlibCrawler != null ? await zlibCrawler.SearchBooksAsync(keyword) : new List<Book>();
            var openlibResults = openlibCrawler != null ? await openlibCrawler.SearchBooksAsync(keyword) : new List<Book>();

            // 组合最终模型
            var model = new CombinedSearchResult
            {
                UserBooks = userUploaded,
                ZLibraryBooks = zlibResults,
                OpenLibraryBooks = openlibResults
            };

            // 传递给视图的额外数据
            ViewBag.Keyword = keyword;
            ViewBag.SelectedFormat = format;        // 当前选中的格式
            ViewBag.FormatCounts = formatCounts;  // 各格式数量字典

            return View(model);
        }
    }
}
