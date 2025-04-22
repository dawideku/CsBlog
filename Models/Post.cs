using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public string? AppUserId { get; set; } // Klucz obcy

        [ForeignKey("AppUserId")]
        public AppUser? AppUser { get; set; } // Nawigacja
    }
}
