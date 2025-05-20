//Tang Jiongzheng(c3509120)//
using Microsoft.AspNetCore.Mvc;
using JBookCrawler.Interface;
using JBookCrawler.BookSource;

namespace JBook.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookController : ControllerBase
    {
        private readonly ICrawlerInterface _crawler;

        public BookController()
        {
            _crawler = new OpenLibrary();
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest("Keyword is required.");

            try
            {
                var results = await _crawler.SearchBooksAsync(keyword);
                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
    }
}