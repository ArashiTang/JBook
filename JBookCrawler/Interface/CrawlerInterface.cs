//Tang Jiongzheng(c3509120)//
namespace JBookCrawler.Interface
{
    public class BookInfo
    {
        public string Title { get; set; } = "";
        public string Author { get; set; } = "";
        public string Url { get; set; } = "";
        public string Description { get; set; } = "";
    }

    public interface ICrawlerInterface
    {
        Task<List<BookInfo>> SearchBooksAsync(string keyword);
    }
}
