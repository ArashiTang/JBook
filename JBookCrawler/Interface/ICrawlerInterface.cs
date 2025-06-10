//Tang Jiongzheng(c3509120)//
using System.Collections.Generic;
using System.Threading.Tasks;
using JBook.Shared.Models;

namespace JBookCrawler.Interface
{
    public interface ICrawlerInterface
    {
        Task<List<Book>> SearchBooksAsync(string keyword);
    }
}