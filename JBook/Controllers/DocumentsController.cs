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
        public async Task<IActionResult> Bookshelf()
        {
            var Documents = await _context.Documents.ToListAsync();
            return View(Documents);
        }

        // GET: /Bookshelf/Add
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        // POST: /Bookshelf/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add([FromForm] string Title, [FromForm] string Author, IFormFile File)
        {
            if (string.IsNullOrWhiteSpace(Title))
            {
                ModelState.AddModelError(nameof(Title), "标题不能为空");
            }
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

            var Document = new Document { Title = Title, FilePath = path };
            _context.Documents.Add(Document);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
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
            return RedirectToAction(nameof(Index));
        }
    }
}