using System.ComponentModel.DataAnnotations;

namespace MyApp.Models
{
    public class Komentarz
    {
        public int Id { get; set; }

        [Required]
        public string Tresc { get; set; }

        public DateTime DataDodania { get; set; } = DateTime.Now;

        // Relacja do posta
        public int PostId { get; set; }
        public Post Post { get; set; }

        // Relacja do użytkownika
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}
