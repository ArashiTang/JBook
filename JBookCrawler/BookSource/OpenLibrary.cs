//Tang Jiongzheng(c3509120)//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using JBook.Shared.Models;
using JBookCrawler.Interface;

namespace JBookCrawler.BookSource
{
    public class OpenLibrary : ICrawlerInterface
    {
        private readonly HttpClient _httpClient;

        public OpenLibrary()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.UserAgent
                       .ParseAdd("JBookCrawler/1.0 (your_email@example.com)");
        }

        public async Task<List<Book>> SearchBooksAsync(string keyword)
        {
            var encodedKeyword = HttpUtility.UrlEncode(keyword);
            var url = $"https://openlibrary.org/search.json?q={encodedKeyword}"
                    + "&fields=title,author_name,key,cover_i&limit=10";

            try
            {
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                    return new List<Book>();

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<OpenLibraryResult>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                var books = new List<Book>();
                if (result?.Docs != null)
                {
                    foreach (var doc in result.Docs)
                    {
                        books.Add(new Book
                        {
                            Title = doc.Title,
                            Author = doc.AuthorName?.FirstOrDefault() ?? "Unknown",
                            Url = $"https://openlibrary.org{doc.Key}",
                            Description = doc.CoverI.HasValue
                                ? $"Cover available (ID: {doc.CoverI})"
                                : "No cover available"
                        });
                    }
                }

                return books;
            }
            catch
            {
                return new List<Book>();
            }
        }

        // For deserializing JSON
        private class OpenLibraryResult
        {
            public List<OpenLibraryDoc> Docs { get; set; }
        }

        private class OpenLibraryDoc
        {
            public string Title { get; set; }
            public List<string> AuthorName { get; set; }
            public string Key { get; set; }
            public int? CoverI { get; set; }
        }
    }
}