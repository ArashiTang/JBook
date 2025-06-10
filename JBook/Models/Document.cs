namespace JBook.Models
{
    public class Document
    {
        public int id { get; set; }
        public string? Title { get; set; }
        public string? FilePath { get; set; }

        public string? ContentType { get; set; }

        public DateTime UploadedAt { get; set; }

        public string? CoverImage { get; set; }
        public string? Description { get; set; }
    }
}
