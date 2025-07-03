using Microsoft.EntityFrameworkCore;

namespace JBook.Models
{
    public class BookContext : DbContext
    {
        public BookContext(DbContextOptions<BookContext> options)
            : base(options)
        { }
        
        public DbSet<Document> Documents => Set<Document>();
        public DbSet<ReadingSetting> DocumentSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           
            modelBuilder.Entity<Document>().HasData(
                    new Document
                    {
                        id = 1,
                        Title = "Sun Zi's Art of War",
                        FilePath = "uploads/sun_zi_art_of_war.txt",

                    },

                    new Document
                    {
                        id = 2,
                        Title = "Alive",
                        FilePath = "uploads/alive.txt",

                    }
);
        }
    }
}
