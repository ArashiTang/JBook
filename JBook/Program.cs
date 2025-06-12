using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using JBookCrawler.Factory;
using JBookCrawler.BookSource;
using JBook.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BookContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BookContext")));

// Add MVC service
builder.Services.AddControllersWithViews();

// Enable memory cache
builder.Services.AddMemoryCache();

// Register crawler service (note that it is singleton or transient, depending on the needs)
builder.Services.AddTransient<ZLibrary>();
builder.Services.AddTransient<OpenLibrary>();
builder.Services.AddSingleton<CrawlerFactory>();

var app = builder.Build();

// Configure HTTP pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

// Configure default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();


