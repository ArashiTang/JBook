using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using JBookCrawler.Interface;

namespace JBookCrawler.BookSource
{
    public class ZLibrary : ICrawlerInterface
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public async Task<List<BookInfo>> SearchBooksAsync(string keyword)
        {
            var results = new List<BookInfo>();
            string searchUrl = $"https://zh.z-library.sk/s/{Uri.EscapeDataString(keyword)}";

            var response = await _httpClient.GetStringAsync(searchUrl);
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(response);

            // Z-Library 的搜索结果格式会变，这里是初版解析逻辑（需调试调整）
            var nodes = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'resItemBox')]");

            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    var titleNode = node.SelectSingleNode(".//h3/a");
                    var authorNode = node.SelectSingleNode(".//div[@class='authors']");

                    if (titleNode != null)
                    {
                        results.Add(new BookInfo
                        {
                            Title = titleNode.InnerText.Trim(),
                            Url = "https://zh.z-library.sk" + titleNode.GetAttributeValue("href", ""),
                            Author = authorNode?.InnerText.Trim() ?? "",
                            Description = "" // ZLib 没有明显简介字段
                        });
                    }
                }
            }

            return results;
        }
    }
}