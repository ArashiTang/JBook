using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using JBookCrawler.Factory;
using JBookCrawler.BookSource;

var builder = WebApplication.CreateBuilder(args);

// 添加 MVC 服务
builder.Services.AddControllersWithViews();

// 启用内存缓存
builder.Services.AddMemoryCache();

// 注册爬虫服务（注意是单例或瞬时，依据需求）
builder.Services.AddTransient<ZLibrary>();
builder.Services.AddTransient<OpenLibrary>();
builder.Services.AddSingleton<CrawlerFactory>();

var app = builder.Build();

// 配置 HTTP 管道
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

// 配置默认路由
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
