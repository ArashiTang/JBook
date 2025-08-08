//Tang Jiongzheng(c3509120)//
using JBook.Shared.Models;

namespace JBookCrawler.Interface
{
    public interface ICrawlerInterface
    {
        Task<List<Book>> SearchBooksAsync(string keyword);
    }
}