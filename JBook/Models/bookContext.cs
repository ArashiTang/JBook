using Microsoft.EntityFrameworkCore;

namespace JBook.Models
{
    public class BookContext : DbContext
    {
        public BookContext(DbContextOptions<BookContext> options)
            : base(options)
        { }
        
        public DbSet<Document> Documents => Set<Document>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           
            modelBuilder.Entity<Document>().HasData(
                    new Document
                    {
                        id = 1,
                        Title = "Sun Zi's Art of War",

                    },

                    new Document
                    {
                        id = 2,
                        Title = "Alive",

                    }
);
        }
    }
}
