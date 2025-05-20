//Tang Jiongzheng(c3509120)//
namespace JBookCrawler.Interface
{
    public interface ICrawlerInterface
    {
        Task<List<Book>> SearchBooksAsync(string keyword);
    }

    public class Book
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string DetailUrl { get; set; }
        public string CoverUrl { get; set; }
    }
}