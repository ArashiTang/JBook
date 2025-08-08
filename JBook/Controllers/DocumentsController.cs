using Microsoft.AspNetCore.Mvc;
using JBook.Models;
using Microsoft.EntityFrameworkCore;
using VersOne.Epub;

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
        public async Task<IActionResult> Add(
            [FromForm] string Title,
            [FromForm] string Author,
            IFormFile File)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            string path = null;
            string contentType = null;
            if (File != null && File.Length > 0)
            {
                string uploadDir = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadDir);

                string ext = Path.GetExtension(File.FileName).ToLower();
                string fileName = $"{Guid.NewGuid()}{ext}";
                path = Path.Combine("uploads", fileName);
                string fullPath = Path.Combine(_env.WebRootPath, path);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await File.CopyToAsync(stream);
                }

                contentType = File.ContentType;
            }

            var document = new Document
            {
                Title = Title,
                Author = Author,
                FilePath = path,
                ContentType = contentType
            };
            _context.Documents.Add(document);
            await _context.SaveChangesAsync();  

            if (!string.IsNullOrEmpty(path) &&
                Path.GetExtension(path).Equals(".epub", StringComparison.OrdinalIgnoreCase))
            {
                string epubFullPath = Path.Combine(_env.WebRootPath, path);
                using (FileStream epubStream = System.IO.File.OpenRead(epubFullPath))
                {
                    EpubBook epubBook = await EpubReader.ReadBookAsync(epubStream);
                    if (epubBook.CoverImage != null)
                    {
                        string coverDir = Path.Combine(_env.WebRootPath, "covers");
                        Directory.CreateDirectory(coverDir);

                        string coverFileName = $"{document.Id}.jpg";
                        string coverRelPath = Path.Combine("covers", coverFileName).Replace("\\", "/");
                        string coverFullPath = Path.Combine(_env.WebRootPath, coverRelPath);
                        if (epubBook.CoverImage != null)
                        {
                            await System.IO.File.WriteAllBytesAsync(coverFullPath, epubBook.CoverImage);
                        }

                        document.CoverPath = coverRelPath;
                        _context.Documents.Update(document);
                        await _context.SaveChangesAsync();
                    }
                }
            }

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