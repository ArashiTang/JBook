using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.ComponentModel.DataAnnotations;


namespace JBook.Models
{
    public class ReadingSetting
    {

        //public string UserId { get; set; }
        [Key]
        [ForeignKey("Document")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]

        public int DocumentId { get; set; }    


        public string Theme { get; set; }     
        public string BgColor { get; set; }     
        public int FontSize { get; set; }      
        public int LastPage { get; set; }

        public int Brightness { get; set; }
    }
}
