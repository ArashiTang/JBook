// Tang Jiongzheng(c3509120)//
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using JBook.Shared.Models;
using JBookCrawler.Interface;

namespace JBookCrawler.BookSource
{
    public class ZLibrary : ICrawlerInterface
    {
        private const string BaseUrl = "https://zh.z-library.sk";
        private readonly HttpClient _httpClient;

        public ZLibrary()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add(
                "User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
        }

        public async Task<List<Book>> SearchBooksAsync(string keyword)
        {
            var books = new List<Book>();
            var searchUrl = $"{BaseUrl}/s/{Uri.EscapeDataString(keyword)}";

            try
            {
                var html = await _httpClient.GetStringAsync(searchUrl);
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                // Suchergebnisse-Container
                var nodes = doc.DocumentNode.SelectNodes(
                    "//div[contains(@class,'resItemBox')]");

                if (nodes != null)
                {
                    foreach (var node in nodes)
                    {
                        var titleNode = node.SelectSingleNode(
                            ".//a[contains(@class,'result-title')]");
                        var authorNode = node.SelectSingleNode(
                            ".//div[contains(@class,'authors')]");
                        var formatNode = node.SelectSingleNode(
                            ".//div[contains(@class,'property_value')]");

                        var title = titleNode?.InnerText.Trim() ?? "Untitled";
                        var author = authorNode?.InnerText.Trim() ?? "Unknown Author";
                        var format = formatNode?.InnerText.Trim() ?? "Unknown Format";
                        var href = titleNode?.GetAttributeValue("href", "");

                        if (!string.IsNullOrEmpty(href))
                        {
                            books.Add(new Book
                            {
                                Title = title,
                                Author = author,
                                Url = BaseUrl + href,
                                Description = $"Format: {format}"
                            });
                        }
                    }
                }

                return books;
            }
            catch
            {
                return new List<Book>();
            }
        }
    }
}