using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JBook.Models
{
    public class BookLink
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// Book title, up to 100 characters
        [Required,MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        /// Author, up to 100 characters
        [MaxLength(100)]
        public string Author { get; set; } = string.Empty;

        /// Format, up to 100 characters
        [MaxLength(100)]
        public string Format { get; set; } = string.Empty;

        /// Description, up to 100 characters
        [MaxLength(100)]
        public string Description { get; set; } = string.Empty;

        /// External Link, up to 500 characters
        [Required, MaxLength(500)]
        public string Url { get; set; } = string.Empty;
    }
}