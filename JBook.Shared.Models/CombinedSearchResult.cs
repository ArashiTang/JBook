using System.Collections.Generic;

namespace JBook.Shared.Models
{
    public class CombinedSearchResult
    {
        public List<Book> UserBooks { get; set; } = new();
        public List<Book> ZLibraryBooks { get; set; } = new();
        public List<Book> OpenLibraryBooks { get; set; } = new();
    }
}