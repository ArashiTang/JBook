using Microsoft.EntityFrameworkCore;

namespace JBook.Models
{
    public class BookContext : DbContext
    {
        public BookContext(DbContextOptions<BookContext> options)
            : base(options)
        { }

        public DbSet<Document> Documents { get; set; }
        public DbSet<ReadingSetting> ReadingSettings { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<BookLink> BookLinks { get; set; }
        public DbSet<Report> Reports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique Index
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Nickname).IsUnique();
            modelBuilder.Entity<Admin>()
                .HasIndex(a => a.Nickname).IsUnique();

            // Admin Account Seed (Test)
            modelBuilder.Entity<Admin>().HasData(
                new Admin { Id = 1, Email = "123456@admin.com", Nickname = "AdminJiaWei", Password = "123456" }
            );

            // User Account Seed (Test)
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Email = "114514@user.com", Nickname = "xiatounan", Password = "114514" }
            );

            // Document Seed (Test)
            modelBuilder.Entity<Document>().HasData(
                new Document { Id = 1, Title = "A Song of Ice and Fire", Author = "Martin, George R.R.", FilePath = "uploads\\A Song of Ice and Fire.epub"
                ,ContentType = "application/epub",CoverPath = "covers/1.jpg"
                }
            );

            modelBuilder.Entity<Document>().HasData(
                new Document
                {
                    Id = 2,
                    Title = "The Count of Monte Cristo",
                    Author = "Victor Hugo",
                    FilePath = "uploads\\The Count of Monte Cristo.txt"
                ,
                    ContentType = "text/plain",
                    CoverPath = null
                }
            );

            modelBuilder.Entity<Document>().HasData(
                new Document
                {
                    Id = 3,
                    Title = "WEEK 5",
                    Author = "None",
                    FilePath = "uploads\\Week 5 - How to work with Views_V02.pdf"
                ,
                    ContentType = "application/pdf",
                    CoverPath = null
                }
            );

            modelBuilder.Entity<Document>().HasData(
                new Document
                {
                    Id = 4,
                    Title = "ReadMe",
                    Author = "JBMan",
                    FilePath = "uploads\\ReadMe.txt"
                ,
                    ContentType = "text/plain",
                    CoverPath = null
                }
            );

            // BookLink Seed (Test)
            modelBuilder.Entity<BookLink>().HasData(
                new BookLink
                {
                    Id = 1,
                    Title = "Harry Potter and The Cursed Child",
                    Author = "J.K.Rowling",
                    Format = "EPUB",
                    Description = "Harry Potter #8",
                    Url = "https://zh.z-library.sk/book/4288516/460399/harry-potter-and-the-cursed-child-harry-potter-8.html"
                }
            );
        }
    }
}
