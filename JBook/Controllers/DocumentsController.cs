using Microsoft.AspNetCore.Mvc;
using JBook.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace JBook.Controllers
{
    [Route("[controller]/[action]")]
    public class DocumentController : Controller
    {
        private readonly BookContext _context;
        private readonly IWebHostEnvironment _env;

        public DocumentController(BookContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: /Bookshelf

        [HttpGet]
        public async Task<IActionResult> Bookshelf()
        {
            var Documents = await _context.Documents.ToListAsync();
            return View(Documents);
        }

        // GET: /Bookshelf/Add

        public IActionResult Add()
        {
            return View();
        }

        // POST: /Bookshelf/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add([FromForm] string Title, [FromForm] string Author, IFormFile File)
        {

            if (!ModelState.IsValid)
                return View();

            string path = null;
            if (File != null && File.Length > 0)
            {
                var uploadDir = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadDir);
                var fileName = Path.GetFileName(File.FileName);
                path = Path.Combine("uploads", fileName);
                using var stream = new FileStream(Path.Combine(_env.WebRootPath, path), FileMode.Create);
                await File.CopyToAsync(stream);
            }

            var Document = new Document { Title = Title, FilePath = path, Author = Author };
            _context.Documents.Add(Document);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Bookshelf));
        }

        // POST: /Bookshelf/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var document = await _context.Documents.FindAsync(id);
            if (document != null)
            {
                _context.Documents.Remove(document);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Bookshelf));
        }

        public async Task<IActionResult> Read(int id)
        {
            var document = await _context.Documents.FindAsync(id);
            if (document == null)
            {
                return NotFound();
            }
            var filePath = Path.Combine(_env.WebRootPath, document.FilePath);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }
            var ext = Path.GetExtension(document.FilePath)
              .TrimStart('.')
              .ToLower();
            if (ext == "pdf")
            {
                return PhysicalFile(filePath, "application/pdf");
            }
            else if (ext == "epub")
            {
                ViewBag.FileUrl = Url.Content($"~/uploads/{Path.GetFileName(document.FilePath)}");
                ViewBag.FileExt = ext;
                ViewBag.DocumentId = document.Id;
                        return View(document);
            }
            var content = await System.IO.File.ReadAllTextAsync(filePath);
            ViewBag.Title = document.Title;
            ViewBag.Author = document.Author;
            var fileName = Path.GetFileName(document.FilePath);
            ViewBag.FileUrl = Url.Content($"~/uploads/{fileName}");
            ViewBag.FileExt = ext;
            return View(document);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSettings(int id)
        {
            var setting = await _context.ReadingSettings
                                .FirstOrDefaultAsync(s => s.DocumentId == id);

            if (setting == null)
            {
                return Json(new
                {
                    theme = "light",
                    bgColor = "#ffffff",
                    fontSize = 16,
                    lastPage = 0,
                    cfi = ""
                });
            }
            return Json(new
            {
                theme = setting.Theme,
                bgColor = setting.BgColor,
                fontSize = setting.FontSize,
                lastPage = setting.LastPage,
                cfi = setting.Cfi
            });
        }


        [HttpPost]
        public async Task<IActionResult> SaveSettings([FromBody] ReadingSetting dto)
        {
            var setting = await _context.ReadingSettings
                                .FirstOrDefaultAsync(s => s.DocumentId == dto.DocumentId);

            if (setting == null)
            {
                _context.ReadingSettings.Add(dto);
            }
            else
            {
                setting.Theme = dto.Theme;
                setting.BgColor = dto.BgColor;
                setting.FontSize = dto.FontSize;
                setting.LastPage = dto.LastPage;
                setting.Cfi = dto.Cfi;
            }
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}