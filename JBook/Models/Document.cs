
namespace JBook.Models
{
    public class Document
    {
        public int Id { get; set; }
        public string? Title { get; set; }

        public string? Author { get; set; }
        public string? FilePath { get; set; }

        public string? ContentType { get; set; }

        //public DateTime UploadedAt { get; set; }

        public string? CoverPath { get; set; }
        //public string? CoverImage { get; set; }

        //public string? Description { get; set; }

        public ReadingSetting? ReadingSetting{ get; set; }
    }
}
