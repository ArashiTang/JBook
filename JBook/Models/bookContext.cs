using Microsoft.EntityFrameworkCore;

namespace JBook.Models
{
    public class BookContext : DbContext
    {
        public BookContext(DbContextOptions<BookContext> options)
            : base(options)
        { }
        public DbSet<Book> Books => Set<Book>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>().HasData(
                                new Book
                                {
                                    bookId = 1,
                                    title = "Sun Zi's Art of War",
                                    author = "Wu Sun",
                                    ISBN = "9787552220094",
                                },

                                new Book
                                {
                                    bookId = 2,
                                    title = "Alive",
                                    author = "Hua Yu",
                                    ISBN = "9787530221532",
                                }
            );
        }
    }
}
