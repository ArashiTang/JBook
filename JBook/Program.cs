using JBookCrawler.Factory;
using JBookCrawler.BookSource;
using JBook.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.StaticFiles;

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

// Configure cookie authentication
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";   // Redirect here if not authenticated
        options.LogoutPath = "/Account/Logout";  // Redirect after logout
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
        options.SlidingExpiration = true;
    });

var app = builder.Build();

var provider = new FileExtensionContentTypeProvider();
provider.Mappings[".epub"] = "application/epub+zip";
app.UseStaticFiles(new StaticFileOptions { ContentTypeProvider = provider });

// Configure HTTP pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Configure default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();