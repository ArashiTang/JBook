using JBookCrawler.BookSource;
using JBookCrawler.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace JBookCrawler.Factory
{
    public class CrawlerFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public CrawlerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ICrawlerInterface? GetCrawler(string source)
        {
            return source.ToLower() switch
            {
                "openlibrary" => _serviceProvider.GetRequiredService<OpenLibrary>(),
                "zlibrary" => _serviceProvider.GetRequiredService<ZLibrary>(),
                _ => null
            };
        }
    }
}