using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace JBook.Models
{
    public class BookContextFactory : IDesignTimeDbContextFactory<BookContext>
    {
        public BookContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<BookContext>();
            var connectionString = config.GetConnectionString("BookContext");

            optionsBuilder.UseSqlServer(connectionString);

            return new BookContext(optionsBuilder.Options);
        }
    }
}