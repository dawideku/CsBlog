using System.ComponentModel.DataAnnotations;

namespace MyApp.Models
{
    public class Post
    {
        public int Id { get; set; }

        [Required]
        public string Tytul { get; set; }

        [Required]
        public string Tresc { get; set; }

        public DateTime DataDodania { get; set; } = DateTime.Now;
    }
}
