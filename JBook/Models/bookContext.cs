using Microsoft.EntityFrameworkCore;

namespace JBook.Models
{
    public class bookContext : DbContext
    {
        public bookContext(DbContextOptions<bookContext> options)
            : base(options)
        { }
        public DbSet<book> books { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<book>().HasData(
                                new book
                                {
                                    bookId = 1,
                                    title = "Sun Zi's Art of War",
                                    author = "Wu Sun",
                                    ISBN = "9787552220094",
                                },

                                new book
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
