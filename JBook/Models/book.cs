using System;
using System.ComponentModel.DataAnnotations;

namespace JBook.Models
{
    public class book
    {
        public int bookId { get; set; }
        public string? title { get; set; }
        public string? author { get; set; }
        public string? publisher { get; set; } 
        public string? category { get; set; }
        public string? description { get; set; }
        public string? ISBN { get; set; }
    }
}
