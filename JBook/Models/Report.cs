using System;
using System.ComponentModel.DataAnnotations;

namespace JBook.Models
{
    public class Report
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Message { get; set; } = string.Empty;

        [Required]
        public DateTime ReportedAt { get; set; }
    }
}